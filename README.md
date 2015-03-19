# snaketail-net
Tail utility for monitoring text log files and Windows EventLog

- Monitor "large" text log files
- Monitor Windows Event Logs (Without needing administrator rights)
- Multiple Window Modes supported (MDI, Tabbed, Floating)
- Save and load entire window session. Can load session file at startup when given as command line parameter.
- Sentence highlight with colors based on keyword match (Includes regex support)
- Quickly jump between highlighted sentences using keyboard shortcuts
- Toggle bookmarks and quickly jump between bookmarks
- Configure external tools and bind custom shortcut key (Trigger execute on highlight)
- Tails circular logs where the log file is periodically truncated/renamed
- Tails log directory where the latest log file is displayed (Includes wildcards)
- Search in the entire text log file (or Windows EventLog)
- Highlight window tabs using icons, when file changes are detected
- Tail new log files with a simple drag drop from Windows Explorer
- Filtering of Windows Event Logs using regular expressions
- Display simple process statistics in window title bar (RAM + CPU usage + TRX/Sec)
- Stop and start services directly
- Change tail window background color
- Change tail window text color
- Change tail window icon
- Minimize to tray
- Low memory usage independent of log file size
- Low cpu usage even when more than 100 lines/sec
- Works well over remote desktop
- Supports Windows 2000, XP, 2003, Vista, Win2k8, Win7
- Requires .NET 2.0
- GNU GPL License v3
