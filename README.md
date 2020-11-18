# WebSocketMockServer
Service that helps frontend team to test web socket integration when backend is not ready with predefined request/response or request/responses.

First version with hardcoded request/response data

App config contains section to add default mock request/responses

```
"MockTemplatesConfiguration": {
    "Templates": {
      "RequestA": [
        "RequestA-Response1",
        "RequestA-Response2"
      ]
    }
```
![example](Example.PNG)
