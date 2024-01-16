using APPDATA.Models;
using APPVIEW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Policy;

namespace APPVIEW.Services
{
    public class SendEmailMessage
    {
        //public string SendProduct(BillDetail)
        //{

        //}

        public string SendEmail(string name, string email, string phone)
        {

            if (name != null && email != null && phone != null)
            {

                string body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"" rel=""stylesheet"">
    <link href=""https://getbootstrap.com/docs/5.3/assets/css/docs.css"" rel=""stylesheet"">
    <title>Tài Khoản Tạo Thành Công</title>
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js""></script>
    <title>Tài Khoản Admin Tạo Thành Công</title>
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
            color: black;
        }}
        .container {{
            width: 80%;
            margin: auto;
            overflow: hidden;
        }}
        header {{
            background: #7047C8;
            color: white;
            padding-top: 30px;
            min-height: 70px;
            border-bottom: #f8c6f3 4px solid;
        }}
        header a {{
            color: #ffffff;
            text-decoration: none;
            text-transform: uppercase;
            font-size: 16px;
        }}
        header ul {{
            padding: 0;
            margin: 0;
            list-style: none;
            overflow: hidden;
        }}
        header #logo {{
            text-align: center;
            margin: 0;
        }}
        header #logo img {{
            width: 70px;
            height: 70px;
        }}
        header h1 {{
            margin-top: 10px;
            margin-bottom: 10px;
        }}
        header #logo h1 {{
            display: inline;
            text-transform: uppercase;
            font-size: 2em;
            margin-top: 40px;
            margin-bottom: 10px;
        }}
        header #logo span {{
            color: #f8c6f3;
        }}
        header a:hover {{
            color: #ffffff;
            text-decoration: underline;
        }}
        header #menu-icon {{
            display: none;
        }}
        section {{
            float: left;
            width: 60%;
            margin: 20px 0 40px 0;
            padding: 20px;
            box-sizing: border-box;
            background: #ffffff;
            border-radius: 5px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
        }}
        footer {{
            float: left;
            width: 100%;
            background: #f8c6f3;
            color: white;
            text-align: center;
            padding: 10px 0;
            position: relative;
            margin-top: 40px;
            border-top: #7047C8 4px solid;
        }}
        @media (max-width: 768px) {{
            header #menu-icon {{
                display: block;
                color: white;
                background: #7047C8;
                text-align: center;
                cursor: pointer;
                font-size: 18px;
            }}
            header a {{
                display: none;
                color: #ffffff;
                padding: 15px 0;
                text-align: center;
                border-bottom: 1px solid #ffffff;
            }}
            header #menu-icon:after {{
                content: 'Menu';
            }}
            header #menu-icon.active:after {{
                content: 'Close';
            }}
            header ul:active, header ul:focus {{
                display: block;
            }}
            header ul {{
                display: none;
                padding: 0;
                margin: 0;
                list-style: none;
            }}
            header li {{
                display: block;
                text-align: center;
                margin-bottom: 15px;
            }}
        }}
    </style>
</head>
<body>
    <header>
        <div class=""container"">
            <div id=""logo"">
                <img src=""~/ContentWebb/img/logo.png"" alt=""Logo"">
                <h1><span>Super</span> Fashion</h1>
            </div>
        </div>
    </header>
    <section>
        <h2>Tài Khoản Admin Đã Được Tạo Thành Cônng Bới Quản Trị viên</h2>
        <p>Xin chào {name},</p>
        <p>Cảm ơn bạn đã tạo tài khoản Admin trên trang web Super Fashion. Dưới đây là thông tin đăng nhập của bạn:</p>
        <p><strong>Tài Khoản:</strong> {email}</p>
        <p><strong>Mật Khẩu Tạm Thời:</strong> {phone}</p>
        <p>Hãy đăng nhập vào trang Admin tại đường link sau: <a href='https://localhost:7095/Account/Login'>Trang Admin Super Fashion</a></p>
        <p><strong>Lưu ý:</strong> Đây là mật khẩu tạm thời, vui lòng đổi mật khẩu ngay sau khi đăng nhập để tăng tính bảo mật cho tài khoản của bạn.</p>
        <p>Xin cảm ơn và chúc bạn có trải nghiệm tuyệt vời trên Super Fashion!</p>
        <p>Trân trọng,<br>Super Fashion Team</p>
    </section>
    <footer>
        <p>&copy; 2023 Super Fashion. All rights reserved.</p>
    </footer>
    <script>
        document.getElementById('menu-icon').onclick = function () {{
            var x = document.getElementsByTagName('ul')[0];
            if (x.style.display === 'block') {{
                x.style.display = 'none';
            }} else {{
                x.style.display = 'block';
            }}
        }};
    </script>
</body>
</html>
";
                return body;
            }
            return "Null";
        }
        public string SendEmailProduct(string name, string email, string phone)
        {
            if (name != null && email != null && phone != null)
            {
                string body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"" rel=""stylesheet"">
    <link href=""https://getbootstrap.com/docs/5.3/assets/css/docs.css"" rel=""stylesheet"">
    <title>Thông Tin Đơn Hàng</title>
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js""></script>
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
            color: black;
        }}
        .container {{
            width: 80%;
            margin: auto;
            overflow: hidden;
        }}
        header {{
            background: #7047C8;
            color: white;
            padding-top: 30px;
            min-height: 70px;
            border-bottom: #f8c6f3 4px solid;
        }}
        header a {{
            color: #ffffff;
            text-decoration: none;
            text-transform: uppercase;
            font-size: 16px;
        }}
        header ul {{
            padding: 0;
            margin: 0;
            list-style: none;
            overflow: hidden;
        }}
        header #logo {{
            text-align: center;
            margin: 0;
        }}
        header #logo img {{
            width: 70px;
            height: 70px;
        }}
        header h1 {{
            margin-top: 10px;
            margin-bottom: 10px;
        }}
        header #logo h1 {{
            display: inline;
            text-transform: uppercase;
            font-size: 2em;
            margin-top: 40px;
            margin-bottom: 10px;
        }}
        header #logo span {{
            color: #f8c6f3;
        }}
        header a:hover {{
            color: #ffffff;
            text-decoration: underline;
        }}
        header #menu-icon {{
            display: none;
        }}
        section {{
            float: left;
            width: 60%;
            margin: 20px 0 40px 0;
            padding: 20px;
            box-sizing: border-box;
            background: #ffffff;
            border-radius: 5px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
        }}
        footer {{
            float: left;
            width: 100%;
            background: #f8c6f3;
            color: white;
            text-align: center;
            padding: 10px 0;
            position: relative;
            margin-top: 40px;
            border-top: #7047C8 4px solid;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        th, td {{
            border: 1px solid #dddddd;
            text-align: left;
            padding: 8px;
        }}
        th {{
            background-color: #7047C8;
            color: white;
        }}
        @media (max-width: 768px) {{
            header #menu-icon {{
                display: block;
                color: white;
                background: #7047C8;
                text-align: center;
                cursor: pointer;
                font-size: 18px;
            }}
            header a {{
                display: none;
                color: #ffffff;
                padding: 15px 0;
                text-align: center;
                border-bottom: 1px solid #ffffff;
            }}
            header #menu-icon:after {{
                content: 'Menu';
            }}
            header #menu-icon.active:after {{
                content: 'Close';
            }}
            header ul:active, header ul:focus {{
                display: block;
            }}
            header ul {{
                display: none;
                padding: 0;
                margin: 0;
                list-style: none;
            }}
            header li {{
                display: block;
                text-align: center;
                margin-bottom: 15px;
            }}
        }}
    </style>
</head>
<body>
    <header>
        <div class=""container"">
            <div id=""logo"">
                <img src=""~/ContentWebb/img/logo.png"" alt=""Logo"">
                <h1><span>Super</span> Fashion</h1>
            </div>
        </div>
    </header>
    <section>
        <h2>Xin chào {name} cảm ơn bạn đã sử dụng dịch vụ của chúng tôi</h2>
        <h2>Thông Tin Đơn Hàng</h2>
        <table>
            <tr>
                <th>Sản Phẩm</th>
                <th>Số Lượng</th>
                <th>Giá</th>
            </tr>
            <tr>
                <td>Áo Polo</td>
                <td>2</td>
                <td>$30.00</td>
            </tr>
            <tr>
                <td>Quần Jean</td>
                <td>1</td>
                <td>$50.00</td>
            </tr>
            <tr>
                <td>Giày Sneaker</td>
                <td>1</td>
                <td>$40.00</td>
            </tr>
        </table>
        <p>Xin cảm ơn bạn đã đặt hàng từ Super Fashion. Dưới đây là chi tiết đơn hàng của bạn:</p>
        <p><strong>Tổng Tiền:</strong> $120.00</p>
        <p>Chúng tôi sẽ liên hệ với bạn để xác nhận đơn hàng và thông tin giao hàng trong thời gian sớm nhất.</p>
        <p>Xin cảm ơn và chúc bạn có trải nghiệm tuyệt vời trên Super Fashion!</p>
        <p>Trân trọng,<br>Super Fashion Team</p>
    </section>
    <footer>
        <p>&copy; 2023 Super Fashion. All rights reserved.</p>
    </footer>
    <script>
        document.getElementById('menu-icon').onclick = function () {{
            var x = document.getElementsByTagName('ul')[0];
            if (x.style.display === 'block') {{
                x.style.display = 'none';
            }} else {{
                x.style.display = 'block';
            }}
        }};
    </script>
</body>
</html>
";
                return body;
            }
            return "Null";
        }
        public string SendEmailBlock(string name, string email)
        {
            if (name != null && email != null)
            {
                string body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"" rel=""stylesheet"">
    <link href=""https://getbootstrap.com/docs/5.3/assets/css/docs.css"" rel=""stylesheet"">
    <title>Tài Khoản Admin Tạo Thành Công</title>
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js""></script>
    <title>Tài Khoản Admin Tạo Thành Công</title>
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
            color: black;
        }}
        .container {{
            width: 80%;
            margin: auto;
            overflow: hidden;
        }}
        header {{
            background: #7047C8;
            color: white;
            padding-top: 30px;
            min-height: 70px;
            border-bottom: #f8c6f3 4px solid;
        }}
        header a {{
            color: #ffffff;
            text-decoration: none;
            text-transform: uppercase;
            font-size: 16px;
        }}
        header ul {{
            padding: 0;
            margin: 0;
            list-style: none;
            overflow: hidden;
        }}
        header #logo {{
            text-align: center;
            margin: 0;
        }}
        header #logo img {{
            width: 70px;
            height: 70px;
        }}
        header h1 {{
            margin-top: 10px;
            margin-bottom: 10px;
        }}
        header #logo h1 {{
            display: inline;
            text-transform: uppercase;
            font-size: 2em;
            margin-top: 40px;
            margin-bottom: 10px;
        }}
        header #logo span {{
            color: #f8c6f3;
        }}
        header a:hover {{
            color: #ffffff;
            text-decoration: underline;
        }}
        header #menu-icon {{
            display: none;
        }}
        section {{
            float: left;
            width: 60%;
            margin: 20px 0 40px 0;
            padding: 20px;
            box-sizing: border-box;
            background: #ffffff;
            border-radius: 5px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
        }}
        footer {{
            float: left;
            width: 100%;
            background: #f8c6f3;
            color: white;
            text-align: center;
            padding: 10px 0;
            position: relative;
            margin-top: 40px;
            border-top: #7047C8 4px solid;
        }}
        @media (max-width: 768px) {{
            header #menu-icon {{
                display: block;
                color: white;
                background: #7047C8;
                text-align: center;
                cursor: pointer;
                font-size: 18px;
            }}
            header a {{
                display: none;
                color: #ffffff;
                padding: 15px 0;
                text-align: center;
                border-bottom: 1px solid #ffffff;
            }}
            header #menu-icon:after {{
                content: 'Menu';
            }}
            header #menu-icon.active:after {{
                content: 'Close';
            }}
            header ul:active, header ul:focus {{
                display: block;
            }}
            header ul {{
                display: none;
                padding: 0;
                margin: 0;
                list-style: none;
            }}
            header li {{
                display: block;
                text-align: center;
                margin-bottom: 15px;
            }}
        }}
    </style>
</head>
<body>
    <header>
        <div class=""container"">
            <div id=""logo"">
                <img src=""~/ContentWebb/img/logo.png"" alt=""Logo"">
                <h1><span>Super</span> Fashion</h1>
            </div>
        </div>
    </header>
    <section>
        <h2>Tài Khoản Của Bạn Đã Bị Khóa</h2>
        <p>Xin chào,{name}</p>
        <p>Chúng tôi thông báo rằng tài khoản của bạn đã bị khóa do vi phạm một số chính sách của chúng tôi.</p>
        <p>Nếu bạn nghĩ đây có thể là một sự nhầm lẫn hoặc do lỗi từ phía quản trị viên, vui lòng liên hệ với chúng tôi qua số điện thoại sau:</p>
        <p><strong>Số Điện Thoại Hỗ Trợ:</strong> 0123456789</p>
        <p>Trân trọng,</p>
        <p>Đội ngũ hỗ trợ của chúng tôi</p>
    </section>
    <footer>
        <p>&copy; 2023 Super Fashion. All rights reserved.</p>
    </footer>
    <script>
        document.getElementById('menu-icon').onclick = function () {{
            var x = document.getElementsByTagName('ul')[0];
            if (x.style.display === 'block') {{
                x.style.display = 'none';
            }} else {{
                x.style.display = 'block';
            }}
        }};
    </script>
</body>
</html>
";
                return body;
            }
            return "Null";

        }
    }

}

