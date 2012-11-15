#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Timers;
namespace System.IO
{
    /// <summary>
    /// FileActor
    /// </summary>
	public class FileActor
	{
		private FileSystemWatcher _fileSystemWatcher;
		private Timer _timer;
		private Action<string> _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileActor"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="action">The action.</param>
		public FileActor(FileActorArguments arguments, Action<string> action)
		{
			_action = action;
			_fileSystemWatcher = new FileSystemWatcher(arguments.Path, arguments.Filter);
			_fileSystemWatcher.IncludeSubdirectories = arguments.IncludeSubdirectories;
			_fileSystemWatcher.NotifyFilter = arguments.NotifyFilter;
			_fileSystemWatcher.Changed += OnDirectoryChangeDetected;
			_fileSystemWatcher.Created += OnDirectoryChangeDetected;
			_fileSystemWatcher.Deleted += OnDirectoryChangeDetected;
			_fileSystemWatcher.Renamed += OnDirectoryChangeDetected;
			_fileSystemWatcher.Error += OnErrorDetected;
			//
			if (arguments.PollInterval.TotalMilliseconds > 0)
			{
				_timer = new Timer(arguments.PollInterval.TotalMilliseconds);
				_timer.Elapsed += TimerElapsed;
			}
		}

        /// <summary>
        /// Starts this instance.
        /// </summary>
		public virtual void Start()
		{
			_fileSystemWatcher.EnableRaisingEvents = true;
			if (_timer != null)
				_timer.Enabled = true;
		}

        /// <summary>
        /// Stops this instance.
        /// </summary>
		public virtual void Stop()
		{
			_fileSystemWatcher.EnableRaisingEvents = false;
			if (_timer != null)
				_timer.Enabled = false;
		}

        /// <summary>
        /// Raises the <see cref="E:FileChanged"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
		protected virtual void OnFileChanged(FileSystemEventArgs eventArgs) { }

        /// <summary>
        /// Raises the <see cref="E:FileCreated"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
		protected virtual void OnFileCreated(FileSystemEventArgs eventArgs) { }

        /// <summary>
        /// Raises the <see cref="E:FileDeleted"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
		protected virtual void OnFileDeleted(FileSystemEventArgs eventArgs) { }

        /// <summary>
        /// Raises the <see cref="E:FileRenamed"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
		protected virtual void OnFileRenamed(FileSystemEventArgs eventArgs) { }

        /// <summary>
        /// Called when [file settled].
        /// </summary>
        /// <param name="fullPath">The full path.</param>
		protected virtual void OnFileSettled(string fullPath)
		{
			_action(fullPath);
		}

        /// <summary>
        /// Called when [error].
        /// </summary>
        /// <param name="exception">The exception.</param>
		protected virtual void OnError(Exception exception)
		{
		}

		private void OnDirectoryChangeDetected(object sender, FileSystemEventArgs e)
		{
			switch (e.ChangeType)
			{
				case WatcherChangeTypes.Changed:
                    //OnFileChanged(e);
                    //System.Threading.Thread.Sleep(1000);
                    //OnFileSettled(e.FullPath);
					return;
				case WatcherChangeTypes.Created:
					OnFileCreated(e);
					return;
				case WatcherChangeTypes.Deleted:
					OnFileDeleted(e);
					return;
				case WatcherChangeTypes.Renamed:
					OnFileRenamed(e);
					return;
			}
		}

		private void OnErrorDetected(object sender, ErrorEventArgs e)
		{
			OnError(e.GetException());
		}

		private void TimerElapsed(object sender, ElapsedEventArgs e)
		{
			_timer.Stop();
			// Determine which search option to use based on the file system watcher
			SearchOption searchOption = (_fileSystemWatcher.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			// Get a list of all of the files that are in the directory the file system watcher is monitoring and that match the filter
            foreach (var file in Directory.GetFiles(_fileSystemWatcher.Path, _fileSystemWatcher.Filter, searchOption))
				_action(file);
			_timer.Start();
		}


        // TODO better method name
        /// <summary>
        /// Recursives as spider.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="filePattern">The file pattern.</param>
        /// <param name="action">The action.</param>
		public static void RecursiveAsSpider(string path, string filePattern, Action<string> action)
		{
			foreach (var file in Directory.GetFiles(path, filePattern))
				try
				{
					action(file);
				}
				catch { }
			foreach (var childPath in Directory.GetDirectories(path))
				try
				{
                    RecursiveAsSpider(childPath, filePattern, action);
				}
				catch { }
		}
	}
}
