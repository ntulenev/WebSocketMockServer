![WebSocketMockServer](logo.PNG)
# WebSocketMockServer

### Tool that helps frontend team test web socket integration when backend is not ready.

Project allows to add multiple responses for one request with delay for every of them.

```
"MockTemplatesConfiguration": {
  "Templates": [
    {
      "File": "RequestA.json",
      "Responses": [
        { "File": "Response1.json" },
        {
          "File": "Response2.json",
          "Delay": "5000"
        }
      ]
    }
  ]
}
```

### RequestA.json

```
{
  "Request": "A"
}
```

### Response1.json

```
{
  "Response": "A"
}
```

### Response2.json

```
{
  "Response": "B"
}
```

### Example

![example image](ExampleImage.PNG)

### Plan to add
* Refactor files logic
* Add unit tests
