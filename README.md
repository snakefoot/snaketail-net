# snaketail-net
Tail utility for monitoring text log files and Windows EventLog

- Monitor "large" text log files
- Monitor Windows Event Logs (Without needing administrator rights)
- Multiple Window Modes supported (MDI, Tabbed, Floating)
- Save and load entire window session. Can load session file at startup when given as command line parameter.
- Sentence highlight with colors based on keyword match (Includes regex support)
- Quickly jump between highlighted sentences using keyboard shortcuts (See Alert highlight below)
- Toggle bookmarks and quickly jump between bookmarks
- Configure external tools and bind custom shortcut key (See External tools below)
- Launch external tools on keyword match (Ex. play sound)
- Most Recent Used (MRU) list of files for quickly reopening files
- Tails circular logs where the log file is periodically truncated/renamed
- Tails log directory where the latest log file is displayed (Includes wildcards)
- Search in the entire text log file (or Windows EventLog)
- Highlight window tabs using icons, when file changes are detected (Display tab icon on log change)
- Tail new log files with a simple drag drop from Windows Explorer
- Default configuration for new files, by saving a session file named SnakeTail.xml (See below)
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
- Supports Windows 2000, XP, 2003, Vista, Win2k8, Win7, Win8, Win10, Win11
- Requires .NET 3.5 or later
- GNU GPL License v3

## Default Session Configuration

It is possible to setup a default configuration for new files, so that every time a new file is opened, it will use the same colors, highlight rules, window icon, etc.

SnakeTail resolves default session config by scanning for `SnakeTail.xml` file at the following locations:

1. Application Path/SnakeTail.xml (Same folder as `SnakeTail.exe`)
2. Current User AppData/Roaming/SnakeTail/SnakeTail.xml (Windows Folder: `%APPDATA%\SnakeTail`)
3. Current User AppData/Local/SnakeTail/SnakeTail.xml (Windows Folder: `%LOCALAPPDATA%\SnakeTail`)
4. Common AppData/SnakeTail/SnakeTail.xml (Windows Folder: `%ProgramData%\SnakeTail`)

The `SnakeTail` folder might not exist, and must then be created manually.

Use-case Examples:

- **Personal defaults:** Save the file in the user profile (Roaming or Local), so all new tail windows get your preferred colors and highlight rules.
- **Shared defaults:** Save the file in Common AppData, so all users on the machine get the same defaults.
- **Portable setup:** Save the file next to the application, so the defaults follow the application when copied to another machine or USB drive.
- **Standard log highlighting:** Add highlight rules for keywords like `ERROR` and `WARN`, so every new log file is instantly colored without further setup.

The option **File > Restore Session on Startup** will re-open files from previous session, by automatically saving session to default `SnakeTail.xml` (resolved from the locations above, or created in `%APPDATA%\SnakeTail` when none exists). Custom sessions `.xml` files are opened separately, but normal log files will be opened on top of the auto restored session.

## Keyword highlight

Keyword rules (plain text or regular expression, optionally case sensitive) are configured per window, and decide what should happen when a line matches. Each rule can combine the following actions ("Keyword Match Actions"):

| Action | Description |
|--------|-------------|
| Line Coloring | Show the matching line with a custom text and background color |
| Alert Highlight | Mark the matching line as important (See below) |
| Window Title Counter | Count the matching lines, and show the count as `Hits=` in the window title |
| Launch External Tool | Launch an external tool when a line matches (See below) |

Alert Highlight enables jumping with `ALT+KeyUp` / `ALT+KeyDown` between important events in the log, and using `Display tab icon on log change` it will highlight not-selected tabs with warning/alert icon.

Use-case Examples:

- **Find the errors quickly:** Use Line Coloring + Alert Highlight on `ERROR` and `Exception`, and jump between the highlighted lines using `ALT+KeyUp` / `ALT+KeyDown` instead of scrolling through the log.
- **Play a sound on errors:** Combine with sound notification using External Tool, together with easily jumping between the highlighted lines.
- **Notice problems in background tabs:** With many open windows, the warning icon on the tab shows which log file just got an alert match.
- **Color without noise:** Line Coloring without Alert Highlight is for lines that should stand out visually, but should not be jump targets or trigger the tab warning icon. Ex. color the line green for success (`completed successfully`) or grey for `DEBUG`.
- **Count without alerting:** Use Window Title Counter without Alert Highlight to keep track of how many times something happens (Ex. `Login failed`), without being alerted for each one.

The rules are evaluated in list order, so put the most specific rules first.

## External tools

External tools can be configured and bound to a custom shortcut key. They can also be launched automatically when a keyword matches (Trigger execute on highlight).

Use-case Examples:

- **Play a sound on keyword match:** Using keyword highlight rules to trigger external tool on `ERROR`.
- **Open the log in another application:** Bind a shortcut key to launch a text editor or diff tool on the current file.
- **Truncate or delete the active log file:** Bind a shortcut key to a tool that clears the file currently being tailed.
- **Run diagnostics on demand:** Bind a shortcut key to a script that collects information from the system being monitored.
- **React to log events:** Launch a script that restarts a service or sends a notification when a specific keyword is detected.

The following tokens can be used in `Command`, `Arguments` and `Initial directory`. They are replaced with their value when the tool is launched. Environment variables (Ex. `%TEMP%`) are also expanded.

| Token | Description |
|-------|-------------|
| `$(FilePath)` | Full path of the file currently being tailed (For rolling filenames and wildcards it is the file currently displayed) |
| `$(FileDirectory)` | Directory of the file currently being tailed |
| `$(FileName)` | File name (with extension) of the file currently being tailed |
| `$(LineText)` | Text of the matching line when launched by keyword match, or the selected line when launched manually |
| `$(LineNumber)` | Line number of `$(LineText)` |
| `$(KeywordText)` | The keyword that matched. Empty when launched manually |
| `$(ViewName)` | Title of the tail window |
| `$(ServiceName)` | Name of the monitored service (Empty when not monitoring a service) |
| `$(ServiceMachineName)` | Machine name of the monitored service (Empty when not monitoring a service) |
| `$(SessionPath)` | Full path of the currently open session file (Empty when no session is open) |
| `$(SessionDirectory)` | Directory of the session file the window was loaded from |
| `$(SessionFileName)` | File name (with extension) of the currently open session file |
| `$(SessionName)` | File name (without extension) of the currently open session file |
| `$(ProgramDirectory)` | Directory where SnakeTail is installed |
