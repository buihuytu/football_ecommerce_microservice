using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Common.Logging
{
    public static class Logging 
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
            (context, loggerConfiguration) =>
            {
                // -- Cho phép truy cập vào thông tin về môi trường, ví dụ như tên môi trường (Development, Production) và tên ứng dụng.--
                var env = context.HostingEnvironment;
                // -- Cấu hình Serilog để ghi log ra Console:--
                // ---- Thiết lập mức độ log mặc định là Information. Điều này có nghĩa là chỉ các sự kiện với mức độ Information, Warning, Error, Fatal mới được ghi lại. Các sự kiện có mức độ thấp hơn như Verbose hay Debug sẽ không được ghi lại trừ khi có cấu hình khác. ---- 
                loggerConfiguration.MinimumLevel.Information()
                    // ------ Thêm thông tin về ngữ cảnh vào log. Ví dụ: thông tin về Request ID hoặc User ID khi có thể. ------
                    .Enrich.FromLogContext()
                    // ------ Thêm một thuộc tính vào mỗi log, cụ thể là tên ứng dụng (ApplicationName). ------
                    .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                    // ------ Thêm thông tin môi trường (Development, Staging, Production,...) vào log. ------
                    .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                    // ------ Khi một lỗi (exception) xảy ra, nó sẽ thêm thông tin chi tiết về lỗi đó vào log. ------
                    .Enrich.WithExceptionDetails()
                    // ------ Thiết lập mức độ log riêng cho các log liên quan đến ASP.NET Core, trong đó chỉ ghi lại các log với mức độ Warning trở lên (bỏ qua Information, Debug, và Verbose).
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    // ------ Thiết lập mức độ log riêng cho các log liên quan đến vòng đời của ứng dụng (ví dụ: thông báo về ứng dụng khởi động, tắt, v.v.), trong đó chỉ ghi lại các log với mức độ Warning trở lên (bỏ qua Information, Debug, và Verbose). ------
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                    // ------ Ghi log vào Console: điều này có nghĩa là các log sẽ được hiển thị trên cửa sổ Console (ví dụ: khi chạy ứng dụng trong môi trường phát triển hoặc khi sử dụng Docker). ------
                    .WriteTo.Console();


                // Cấu hình Serilog để ghi log vào file theo định dạng yêu cầu
                var logDirectory = "Logs"; // Thư mục lưu trữ log, bạn có thể thay đổi đường dẫn nếu cần
                loggerConfiguration.WriteTo.File(
                    path: Path.Combine(logDirectory, $"{env.ApplicationName}-{env.EnvironmentName}-Logs-{{0:yyyy.MM.dd}}.log"),
                    rollingInterval: RollingInterval.Day, // Ghi log theo ngày
                    retainedFileCountLimit: 7, // Giới hạn giữ lại 7 ngày log
                    fileSizeLimitBytes: 10 * 1024 * 1024, // Giới hạn kích thước file log (ở đây là 10MB)
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {NewLine}{Exception}"
                );

                // Cấu hình cho môi trường phát triển (Development)
                if (context.HostingEnvironment.IsDevelopment())
                {
                    // Trong môi trường Development, mức độ log được tăng cường cho các mô-đun như Catalog, Basket, Discount, và Ordering. Mức độ Debug được áp dụng cho các mô-đun này, giúp ghi lại nhiều thông tin chi tiết hơn trong quá trình phát triển.
                    loggerConfiguration.MinimumLevel.Override("Catalog", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Basket", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Discount", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Ordering", LogEventLevel.Debug);
                }

                // Ghi log vào Elasticsearch
                // Lấy cấu hình Elasticsearch (từ ElasticConfiguration:Uri trong appsettings.json)
                var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
                if (!string.IsNullOrEmpty(elasticUrl))
                {
                    // Cấu hình Serilog để ghi các log vào Elasticsearch.
                    loggerConfiguration.WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(elasticUrl))
                        {
                            // Tự động đăng ký template (mẫu) trong Elasticsearch.
                            AutoRegisterTemplate = true,
                            // Đảm bảo rằng template phù hợp với phiên bản Elasticsearch v8.
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                            // Thiết lập tên chỉ mục (index) trong Elasticsearch theo định dạng ngày (ví dụ: ECommerce-Logs-2025.08.20)
                            IndexFormat = "ecommerce-logs-{0:yyyy.MM.dd}",
                            // Ghi lại tất cả log có mức độ từ Debug trở lên (bao gồm Debug, Information, Warning, Error, và Fatal)
                            MinimumLogEventLevel = LogEventLevel.Debug
                        });
                }
            };
    }
}
/*
    Thứ tự các mức độ log trong Serilog từ thấp đến cao là: Verbose -> Debug -> Information -> Warning -> Error -> Fatal.
    Dòng mã builder.Host.UseSerilog(Logging.ConfigureLogger); trong Program.cs có ý nghĩa rất quan trọng trong việc cấu hình và sử dụng Serilog trong ứng dụng .NET (thường là ứng dụng ASP.NET Core hoặc .NET Console). Dưới đây là giải thích chi tiết về dòng mã này:
        1. builder.Host.UseSerilog(...)
            - builder là đối tượng IHostBuilder trong ứng dụng .NET, được sử dụng để cấu hình và tạo đối tượng host cho ứng dụng.
            - UseSerilog() là phương thức mở rộng được cung cấp bởi Serilog cho phép cấu hình Serilog làm công cụ ghi log cho ứng dụng.
            - Khi gọi UseSerilog(), Serilog sẽ được khởi tạo và sử dụng để ghi log thay thế cho hệ thống logging mặc định của .NET (chẳng hạn như ILogger).
            - Mục đích: Dòng mã này cấu hình ứng dụng để sử dụng Serilog thay vì hệ thống logging mặc định của .NET. Tức là, thay vì dùng ILogger mặc định, ứng dụng sẽ ghi log thông qua Serilog theo cách mà bạn đã cấu hình.
        2. Logging.ConfigureLogger
            - Logging.ConfigureLogger là một delegate (hàm con) kiểu Action<HostBuilderContext, LoggerConfiguration>.
            - Delegate này được định nghĩa trong lớp Logging (như bạn đã cung cấp ở câu hỏi trước), chứa các cài đặt cấu hình cho Serilog.
            - ConfigureLogger sẽ thiết lập các tùy chọn cho Serilog như mức độ log, nơi ghi log (console, file, Elasticsearch...), và các thông tin bổ sung (ví dụ: môi trường, tên ứng dụng).
            - Ví dụ, trong ConfigureLogger, đã cấu hình:
                + Mức độ log tối thiểu là Information.
                + Ghi log vào console.
                + Cấu hình ghi log vào Elasticsearch nếu có URL Elasticsearch trong cấu hình.
        3. Quá trình chạy
        Khi ứng dụng chạy, quá trình xử lý sẽ như sau:
            - Khởi tạo ứng dụng: Phương thức builder.Host.UseSerilog(Logging.ConfigureLogger); sẽ được gọi trong quá trình khởi tạo ứng dụng. Nó nói với hệ thống .NET là "Hãy sử dụng Serilog để ghi log."
            - Cấu hình Serilog: Logging.ConfigureLogger sẽ được thực thi, với tham số là một đối tượng HostBuilderContext (chứa thông tin về môi trường ứng dụng và cấu hình).
            - context.HostingEnvironment: Cho phép bạn truy cập vào thông tin về môi trường, ví dụ như tên môi trường (Development, Production) và tên ứng dụng.
            - Sau đó, ConfigureLogger sẽ cấu hình Serilog với các cài đặt bạn đã chỉ định, ví dụ như ghi log vào console, vào Elasticsearch, và thêm các thông tin ngữ cảnh vào log (như ApplicationName, EnvironmentName).
            - Kết nối Serilog với Host: Sau khi cấu hình Serilog, Serilog sẽ hoạt động ngay khi ứng dụng bắt đầu, và tất cả các log sẽ được ghi lại theo cấu hình đã định.
            - Ghi log: Sau khi ứng dụng khởi động, khi có sự kiện xảy ra trong ứng dụng, Serilog sẽ bắt đầu ghi log theo mức độ cấu hình (Information, Warning, Error, ...), vào các đích được chỉ định (console, Elasticsearch, v.v.).
        4. Sử dụng:
        - Khi gọi: _logger.LogInformation($"Product with {productName} fetched."):
            + Khi gọi _logger.LogInformation(...), Serilog sẽ xử lý log đó nếu bạn đã cấu hình Serilog như một nhà cung cấp log trong ứng dụng.
            + ILogger<T> chỉ là abstraction, và khi Serilog được cấu hình trong ứng dụng, tất cả các log qua ILogger sẽ tự động được chuyển đến Serilog để xử lý (ghi vào console, file, Elasticsearch, v.v.).
 */