using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumer;
using Order.API.Models;
using Shared;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<OrderAPIDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

builder.Services.AddMassTransit(configuratior =>
{
    configuratior.AddConsumer<PaymentCompletedEventConsumer>();
    configuratior.AddConsumer<StockNotReservedEventConsumer>();
    configuratior.AddConsumer<PaymentFailedEventConsumer>();
    configuratior.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host("RabbitMQ Host adresi");
        _configurator.ReceiveEndpoint(RabbitMQSettings.Order_Payment_Completed_Event_Queue, e => e.ConfigureConsumer<PaymentCompletedEvent>(context));
        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_Consumer, e => e.ConfigureConsumer<StockNotReservedEventConsumer>(context));
        _configurator.ReceiveEndpoint(RabbitMQSettings.Payment_Failed, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
