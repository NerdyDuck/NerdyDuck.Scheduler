#region Copyright
/*******************************************************************************
 * NerdyDuck.Scheduler - Task scheduler for .NET with variable schedules.
 * 
 * The MIT License (MIT)
 *
 * Copyright (c) Daniel Kopp, dak@nerdyduck.de
 *
 * All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 ******************************************************************************/
#endregion

using System.Collections.Generic;
using System.Threading;

namespace NerdyDuck.Scheduler;

public class Scheduler<T> : IDisposable where T : ScheduledActionBase, new()
{
	/// <summary>
	/// The event that is fired before a <see cref="ScheduledTask{T}"/> is executed.
	/// </summary>
	public event EventHandler<ScheduledTaskStartingEventArgs<T>> TaskStarting;

	/// <summary>
	/// The event that is fired after a <see cref="ScheduledTask{T}"/> was executed.
	/// </summary>
	public event EventHandler<ScheduledTaskCompletedEventArgs<T>> TaskCompleted;

	private int _isDisposed;
	private bool _isEnabled;
	private readonly object _syncRoot;
	private TimeSpan _interval;
	private Timer _triggerTimer;
	private List<ScheduledTask<T>> _tasks; // TODO: thread safety!
	private Dictionary<ScheduledTask<T>, CancellationTokenSchedulerAction> _scheduledTasks; // TODO: thread safety!

	/// <summary>
	/// Gets or sets the time interval in which the <see cref="Scheduler{T}"/> checks for tasks that are due.
	/// </summary>
	/// <value>When the interval elapses, all <see cref="Tasks"/> are evaluated if they are due. Must be positive.</value>
	public TimeSpan Interval
	{
		get => _interval;
		set
		{
			AssertDisposed();
			AssertInterval(value);
			_interval = value;
			lock (_syncRoot)
			{
				if (_isEnabled)
				{
					_triggerTimer.Change(_interval, _interval);
				}
			}
		}
	}

	/// <summary>
	/// Gets a value indicating if the <see cref="Scheduler{T}"/> is running.
	/// </summary>
	/// <value>If <see langword="true"/>, the <see cref="Scheduler{T}"/> is active and runs tasks when they are due.</value>
	public bool IsEnabled => _isEnabled;

	/// <summary>
	/// Gets a list of scheduled tasks.
	/// </summary>
	/// <value>The tasks to execute.</value>
	public IList<ScheduledTask<T>> Tasks
	{
		get
		{
			AssertDisposed();
			return _tasks;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Scheduler{T}"/> class.
	/// </summary>
	public Scheduler()
		: this(TimeSpan.FromMinutes(1.0))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Scheduler{T}"/> class with the specified interval.
	/// </summary>
	/// <param name="interval">The time interval in which the <see cref="Scheduler{T}"/> checks for tasks that are due.</param>
	public Scheduler(TimeSpan interval)
	{
		AssertInterval(interval);
		_interval = interval;
		_isDisposed = 0;
		_isEnabled = false;
		_syncRoot = new object();
		_triggerTimer = new Timer(new TimerCallback(TriggerCallback), this, Timeout.Infinite, Timeout.Infinite);
		_tasks = new List<ScheduledTask<T>>();
		_scheduledTasks = new Dictionary<ScheduledTask<T>, CancellationTokenSchedulerAction>();
	}

	/// <summary>
	/// Destructor.
	/// </summary>
	~Scheduler()
	{
		Dispose(false);
	}

	/// <summary>
	/// Starts the <see cref="Scheduler{T}"/>.
	/// </summary>
	public void Start()
	{
		AssertDisposed();
		lock (_syncRoot)
		{
			if (_isEnabled)
				return;

			DateTimeOffset now = DateTimeOffset.Now;
			foreach (ScheduledTask<T> task in _tasks)
			{
				task.UpdateSchedule(now);
			}
			_isEnabled = true;
			_triggerTimer.Change(TimeSpan.Zero, _interval);
		}
	}

	/// <summary>
	/// Stops the <see cref="Scheduler{T}"/> waiting for all tasks to finish or cancel.
	/// </summary>
	public void Stop() => Stop(TimeSpan.Zero);

	/// <summary>
	/// Stops the <see cref="Scheduler{T}"/>, waiting a specified time for all tasks to finish or cancel.
	/// </summary>
	/// <param name="timeout">The maximum time to wait for all tasks to finish or cancel. If <see cref="TimeSpan.Zero"/> is specified, the method waits indefinitely.</param>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
	/// <exception cref="CodedTimeoutException"><paramref name="timeout"/> has elapsed, but there are still tasks running.</exception>
	public void Stop(TimeSpan timeout)
	{
		if (timeout < TimeSpan.Zero)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.Scheduler_Stop_Negative), nameof(timeout), timeout, TextResources.Scheduler_Stop_TimeoutNegative);
		}

		if (_isDisposed == 1)
			return;

		bool waitForTasks = false;
		lock (_syncRoot)
		{
			if (!_isEnabled)
				return;

			_isEnabled = false;
			_triggerTimer.Change(Timeout.Infinite, Timeout.Infinite);
			if (_scheduledTasks.Count > 0)
			{
				CancelAllTasks();
				waitForTasks = true;
			}
		}


		if (waitForTasks)
		{
			bool WaitResult = true;
			if (timeout != TimeSpan.Zero)
			{
				WaitResult = SpinWait.SpinUntil(() => { return _scheduledTasks.Count == 0; }, timeout);
			}
			else
			{
				SpinWait.SpinUntil(() => { return _scheduledTasks.Count == 0; });
			}

			if (!WaitResult)
			{
				throw new CodedTimeoutException(HResult.Create(ErrorCodes.Scheduler_Stop_TasksRunning), TextResources.Scheduler_Stop_Timeout);
			}
		}
	}

	/// <summary>
	/// Releases allocated resources.
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Releases allocated resources.
	/// </summary>
	/// <param name="disposing">A value indicating if the method was called by user code. If <see langword="false"/>, the method was called by the runtime in the finalizer.</param>
	/// <remarks>If <paramref name="disposing"/> is <see langword="false"/>, no other objects should be referenced.</remarks>
	protected virtual void Dispose(bool disposing)
	{
		if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
		{
			return;
		}

		if (disposing)
		{
			_triggerTimer?.Dispose();
			if (_tasks != null)
			{
				foreach (ScheduledTask<T> task in _tasks)
				{
					task.Dispose();
				}
				_tasks.Clear();
			}
			_scheduledTasks?.Clear();
		}
	}
	/// <summary>
	/// Checks if the <see cref="ScheduledTask{T}"/> is disposed.
	/// </summary>
	private void AssertDisposed()
	{
		if (_isDisposed == 1)
		{
			throw new ObjectDisposedException(ToString());
		}
	}

	/// <summary>
	/// Checks if the interval is valid.
	/// </summary>
	/// <param name="interval">The interval to check.</param>
	private static void AssertInterval(TimeSpan interval)
	{
		if (interval < TimeSpan.Zero)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.Scheduler_AssertInterval_Negative), nameof(interval), TextResources.Scheduler_AssertInterval_Negative);
		}
	}

	/// <summary>
	/// Raises the <see cref="TaskStarting"/> event.
	/// </summary>
	/// <param name="task">The <see cref="ScheduledTask{T}"/> that is being executed.</param>
	/// <returns><see langword="true"/>, if the task execution should be canceled; otherwise, <see langword="false"/>.</returns>
	private bool OnTaskStarting(ScheduledTask<T> task)
	{
		ScheduledTaskStartingEventArgs<T> e = new(task);
		try
		{
			TaskStarting?.Invoke(this, e);
		}
		catch { }
		return e.Cancel;
	}

	/// <summary>
	/// Raises the <see cref="TaskCompleted"/> event.
	/// </summary>
	/// <param name="task">The <see cref="ScheduledTask{T}"/> that is was executed.</param>
	/// <param name="action">The result of the execution.</param>
	private void OnTaskCompleted(ScheduledTask<T> task, ISchedulerAction action)
	{
		try
		{
			TaskCompleted?.Invoke(this, new ScheduledTaskCompletedEventArgs<T>(task, action.IsSuccess, action.IsCanceled, action.Exception, action.State));
		}
		catch { }
	}

	/// <summary>
	/// Request the cancellation of all tasks currently active.
	/// </summary>
	private void CancelAllTasks()
	{
		foreach (KeyValuePair<ScheduledTask<T>, CancellationTokenSchedulerAction> pair in _scheduledTasks)
		{
			pair.Value.Cancel();
		}
	}

	/// <summary>
	/// Checks if a task is due, and starts it.
	/// </summary>
	/// <param name="state">The current <see cref="Scheduler{T}"/>.</param>
	private void TriggerCallback(object state)
	{
		if (_isDisposed == 1 || !_isEnabled)
			return;
		lock (_syncRoot)
		{
			foreach (ScheduledTask<T> task in _tasks)
			{
				if (!_isEnabled)
					break;
				if (task.IsTaskDue && !_scheduledTasks.ContainsKey(task))
				{
					CancellationTokenSchedulerAction action = new();
					task.IsScheduled = true;
					_scheduledTasks.Add(task, action);
					ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCallback), new Tuple<ScheduledTask<T>, CancellationTokenSchedulerAction>(task, action));
				}
			}
		}
	}

	/// <summary>
	/// The method that is executed by the thread pool.
	/// </summary>
	/// <param name="state">A state object. Contains the task to execute.</param>
	private void ThreadPoolCallback(object state)
	{
		Tuple<ScheduledTask<T>, CancellationTokenSchedulerAction> Parameters = state as Tuple<ScheduledTask<T>, CancellationTokenSchedulerAction>;
		try
		{
			if (Parameters.Item2.IsCancellationRequested || OnTaskStarting(Parameters.Item1))
			{
				Parameters.Item2.IsCanceled = true;
			}
			else
			{
				Parameters.Item1.Run(Parameters.Item2);
			}
		}
		catch (Exception ex)
		{
			Parameters.Item2.IsSuccess = false;
			Parameters.Item2.IsCanceled = false;
			Parameters.Item2.Exception = ex;
		}
		finally
		{
			if (_isDisposed == 0)
			{
				OnTaskCompleted(Parameters.Item1, Parameters.Item2);
				Dictionary<ScheduledTask<T>, CancellationTokenSchedulerAction> tasks = _scheduledTasks;
				if (tasks != null)
				{
					tasks.Remove(Parameters.Item1);
				}
				Parameters.Item1.UpdateSchedule(DateTimeOffset.Now);
				Parameters.Item1.IsScheduled = false;
				Parameters.Item2.Source.Dispose();
			}
		}
	}

	/// <summary>
	/// Implementation if <see cref="ISchedulerAction"/> for use with a <see cref="CancellationTokenSource"/>.
	/// </summary>
	internal class CancellationTokenSchedulerAction : ISchedulerAction, IDisposable
	{
		private CancellationTokenSource _source;
		private int _isDisposed;

		/// <summary>
		/// Gets whether cancellation has been requested for this action.
		/// </summary>
		/// <value><see langword="true"/>, if the action should be canceled; otherwise, <see langword="false"/>.</value>
		public bool IsCancellationRequested
		{
			get
			{
				AssertDisposed();
				return _source.Token.IsCancellationRequested;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if the action completed successfully.
		/// </summary>
		/// <value><see langword="true"/>, if the action completed successfully; otherwise, <see langword="false"/>.</value>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if the action was canceled or aborted.
		/// </summary>
		/// <value><see langword="true"/>, if the action was canceled; otherwise, <see langword="false"/>.</value>
		public bool IsCanceled { get; set; }

		/// <summary>
		/// Gets or sets the exception that caused the action to fail.
		/// </summary>
		/// <value>If <see cref="IsSuccess"/> and <see cref="IsCanceled"/> is <see langword="false"/>, an <see cref="Exception"/>; otherwise, <see langword="null"/>.</value>
		public Exception Exception { get; set; }

		/// <summary>
		/// Gets or sets an arbitrary state object.
		/// </summary>
		public object State { get; set; }

		/// <summary>
		/// Gets the underlying <see cref="CancellationTokenSource"/>.
		/// </summary>
		public CancellationTokenSource Source => _source;

		/// <summary>
		/// Initializes a new instance of the <see cref="CancellationTokenSchedulerAction"/> class.
		/// </summary>
		public CancellationTokenSchedulerAction()
		{
			_source = new CancellationTokenSource();
			_isDisposed = 0;
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~CancellationTokenSchedulerAction()
		{
			Dispose(false);
		}

		/// <summary>
		/// Throws an exception if the action was canceled.
		/// </summary>
		/// <exception cref="OperationCanceledException"><see cref="IsCancellationRequested"/> is <see langword="true"/>.</exception>
		public void ThrowIfCancellationRequested()
		{
			AssertDisposed();
			_source.Token.ThrowIfCancellationRequested();
		}

		/// <summary>
		/// Sets <see cref="IsCancellationRequested"/> to <see langword="true"/>.
		/// </summary>
		public void Cancel()
		{
			AssertDisposed();
			_source.Cancel();
		}

		/// <summary>
		/// Releases allocated resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases allocated resources.
		/// </summary>
		/// <param name="disposing">A value indicating if the method was called by user code. If <see langword="false"/>, the method was called by the runtime in the finalizer.</param>
		/// <remarks>If <paramref name="disposing"/> is <see langword="false"/>, no other objects should be referenced.</remarks>
		protected virtual void Dispose(bool disposing)
		{
			if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
			{
				return;
			}

			if (disposing)
			{
				_source?.Dispose();
			}
		}

		/// <summary>
		/// Checks if the <see cref="ScheduledTask{T}"/> is disposed.
		/// </summary>
		private void AssertDisposed()
		{
			if (_isDisposed == 1)
			{
				throw new ObjectDisposedException(ToString());
			}
		}
	}
}
