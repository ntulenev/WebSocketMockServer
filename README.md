![WebSocketMockServer](logo.PNG)
# WebSocketMockServer

### Tool that helps frontend team to test web socket integration when backend is not ready.

Project allows to add multiple reactions on one request (responses or delayed notifications).

Current version supports only JSON communication.

### Configuration with request-response mapping
```yaml
"FileLoaderConfiguration": {
  "Folder": "Files",
  "Mapping": [
    {
      "File": "RequestA.json",
      "Reactions": 
      [
        { 
          "File": "Reaction1.json" 
        },
        {
          "File": "Reaction2.json",
          "Delay": "00:00:05"
        }
      ]
    }
  ]
}
```

### RequestA.json

```yaml
{
  "Request": "A"
}
```

### Reaction1.json

```yaml
{
  "Response": "A"
}
```

### Reaction2.json

```yaml
{
  "Notification": "B"
}
```

### Example

![example](Example.png)
