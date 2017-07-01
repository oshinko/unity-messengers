# Unity Messengers
Socket examples and utilities for Unity.

## Launching Server silently
On macOS:

``` bash
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -quit \
  -projectPath ~/Projects/unity-messengers \
  -executeMethod UdpExample.Server.Start \
  -executeMethodArgs 8080
```