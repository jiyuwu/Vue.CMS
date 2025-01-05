using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Core.Filter;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// ʹ�� Autofac �滻Ĭ�ϵķ����ṩ���򹤳�
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add services to the container.
// ���Swagger����������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ���� Kestrel �� IIS ����
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10485760;
});
builder.WebHost.UseKestrel().UseUrls("http://*:9009");
builder.WebHost.UseIIS();

// ��ӷ�������
builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
            builder =>
            {
                builder.AllowAnyOrigin()
               .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
                .AllowAnyHeader().AllowAnyMethod();
            });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllers().AddNewtonsoftJson(op =>
{
    op.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    op.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    op.SerializerSettings.Converters.Add(new LongCovert());
});
//��ʼ�������ļ�
AppSetting.Init(builder.Services, configuration);
//// ���� ConfigureContainer ������ע�� Autofac �����е��Զ���ģ��
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Assuming `Services.AddModule` is an extension method for registering modules
    builder.Services.AddModule(containerBuilder, configuration);
});
builder.Services.UseSqlSugar();

var app = builder.Build();

// �����м��ʹ��Swagger
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //����HttpContext
    app.UseStaticHttpContext();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApi");
        c.RoutePrefix = string.Empty; // ��Swagger UI����ΪӦ�õĸ�·����Ĭ��Ϊ /swagger��
    });
}
// ��ӿ����м��
app.UseCors(); // ȷ��������·���м��֮ǰ������

app.UseHttpsRedirection();
app.UseRouting();
// �����м��
app.UseMiddleware<TokenMiddleware>();
app.UseAuthorization();
app.MapControllers();


app.Run();

