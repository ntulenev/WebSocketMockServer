# WebSocketMockServer

### Service that helps frontend team test web socket integration when backend is not ready.

First version with simple request/response data from appsettings.
Supports delayed notifications.

```
"MockTemplatesConfiguration": {
  "Templates": {
    "RequestA": [
      { "Text": "RequestA-Response1" },
      {
        "Text": "RequestA-Response2",
        "Delay": "5000"
      }
    ]
  }
}
```

### Example

![example](Example.PNG)

### Plan to add
* Move Request/Response in separate json files
