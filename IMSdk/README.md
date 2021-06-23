# im-netcore-sdk
im for netcore

具体使用方法，可查看单元测试项目 /Test 目录下

``` c#
[TestMethod]
public void TestStart()
{
    try
    {
        var base64Secret = "";
        if (base64Secret.Length % 4 > 0)
        {
            base64Secret = base64Secret.PadRight(base64Secret.Length + 4 - base64Secret.Length % 4, '=');
        }

        var outpuBytes = Convert.FromBase64String(base64Secret);
        var rawSecret = Encoding.UTF8.GetString(outpuBytes);
        var im = JsonConvert.DeserializeObject<IMModel>(rawSecret);

        var thirdUID = "test-go-tob";
        var aID = "test";

        var result = new Account(im).GetTokenByThirdUID(thirdUID, aID, "test-go", SexEnum.Unknown, "http://xxxxx/app/rank-list/static/img/defaultavatar.cd935fdb.png", "", "", "");
        if (!string.IsNullOrWhiteSpace(result.Item3))
        {
            Debug.WriteLine(result.Item3);
            return;
        }

        Debug.WriteLine($"token:{result.Item1},openID:{result.Item2}");

        var toUID = "1100";
        var objectName = "RC:textMsg";
        var content = "测试单聊";

        var dataPush = new DataPush(im);
        var res = dataPush.PushConverseData(thirdUID, toUID, objectName, content, "", "", "", "", "", "");
        if (!res.Item1)
        {
            Debug.WriteLine($"PushConverseData exeute fail.{res.Item2 ?? ""}");
            return;
        }

        Debug.WriteLine("PushConverseData exeute finish.");

        content = "测试 事件";
        res = dataPush.PushEventData(thirdUID, toUID, objectName, content, "", "", "", "");
        if (!res.Item1)
        {
            Debug.WriteLine($"PushEventData exeute fail. {res.Item2}");
            return;
        }

        Debug.WriteLine("PushEventData exeute finish.");
    }
    catch (Exception ex)
    {
        Debug.WriteLine(ex.Message);
        Debug.WriteLine(ex.StackTrace);
    }
}
```
