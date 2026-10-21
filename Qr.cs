using System.Drawing;using QRCoder;
namespace PocketBridge {public static class Qr {public static Bitmap Create(string text){using(var generator=new QRCodeGenerator())using(var data=generator.CreateQrCode(text,QRCodeGenerator.ECCLevel.M))using(var qr=new QRCode(data))using(var image=qr.GetGraphic(5)){return new Bitmap(image);}}}}
