using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System;

class SendOTP
{
    public void Send(string sdt)
    {
        string accountSid = "ACf44c1e794dc07bcd64d7a3b8b74648ee";
        string authToken = "a16522fc3c2d415af4715f30e8edcb80";
        string twilioPhoneNumber = "+84337658308";

        TwilioClient.Init(accountSid, authToken);

        string phoneNumber = "+84337658308"; // Số điện thoại người nhận (Việt Nam)

        string otpCode = GenerateRandomOTP();
        string message = $"Your OTP code is: {otpCode}";

        var messageResource = MessageResource.Create(
            body: message,
            from: new PhoneNumber(twilioPhoneNumber),
            to: new PhoneNumber(phoneNumber)
        );

        Console.WriteLine($"Message SID: {messageResource.Sid}");
    }

    public string GenerateRandomOTP()
    {
        // Tạo mã OTP ngẫu nhiên 6 chữ số (có thể sử dụng thuật toán phù hợp)
        Random rand = new Random();
        return rand.Next(100000, 999999).ToString();
    }
}