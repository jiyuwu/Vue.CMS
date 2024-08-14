using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using JIYUWU.Core.DbSqlSugar;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddControllers();
builder.Services.UseSqlSugar();

var app = builder.Build();

// �����м��ʹ��Swagger
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApi");
        c.RoutePrefix = string.Empty; // ��Swagger UI����ΪӦ�õĸ�·����Ĭ��Ϊ /swagger��
    });
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();


app.Run();

