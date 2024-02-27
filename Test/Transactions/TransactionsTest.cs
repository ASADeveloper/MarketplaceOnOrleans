﻿using Common.Entities;
using Common.Requests;
using OrleansApp.Transactional;
using Test.Infra;
using Test.Infra.Transactional;

namespace Test.Transactions;

[Collection(TransactionalClusterCollection.Name)]
public class TransactionsTest : BaseTest
{
    public TransactionsTest(TransactionalClusterFixture fixture) : base(fixture.Cluster){}

    [Fact]
    public async Task TestCheckout()
    {
        int customerId = 1;
        await InitData(1, 2);
        await BuildAndSendCheckout(1);

        var orderActor = _cluster.GrainFactory.GetGrain<ITransactionalOrderActor>(customerId);
        List<Order> orders = await orderActor.GetOrders();

        Assert.Single(orders);

        var shipmentActor = _cluster.GrainFactory.GetGrain<ITransactionalShipmentActor>(0);
        var shipments = await shipmentActor.GetShipments(customerId);

        Assert.Single(shipments);

        // reset to avoid impacting other tests
        await shipmentActor.Reset();
    }

    [Fact]
    public async Task TestDelivery()
    {
        int customerId = 1;
        await this.InitData(1, 2);
        await this.BuildAndSendCheckout(customerId);

        var shipmentActorId = 0;
        var shipmentActor = _cluster.GrainFactory.GetGrain<ITransactionalShipmentActor>(shipmentActorId);

        var shipments = await shipmentActor.GetShipments(customerId);
        Assert.True(shipments.Count == 1);

        await shipmentActor.UpdateShipment(0.ToString());
        shipments = await shipmentActor.GetShipments(customerId);

        Assert.Empty(shipments);
    }

    [Fact]
    public async Task TestPriceUpdate()
    {
        var productActor = _cluster.GrainFactory.GetGrain<ITransactionalProductActor>(1,1.ToString());

        await productActor.SetProduct( new Product()
        {
            seller_id = 1,
            product_id = 1,
            price = 1,
            freight_value = 1,
            active = true,
            version = 1.ToString(),
        });

        PriceUpdate priceUpdate = new PriceUpdate() { price = 10 };

        await productActor.ProcessPriceUpdate(priceUpdate);

        var newPrice = (await productActor.GetProduct()).price;

        Assert.True(newPrice == priceUpdate.price);
    }

    [Fact]
    public async Task TestProductUpdate()
    {
        var productActor = _cluster.GrainFactory.GetGrain<ITransactionalProductActor>(1, 1.ToString());

        await productActor.SetProduct(new Product()
        {
            seller_id = 1,
            product_id = 1,
            price = 1,
            freight_value = 1,
            active = true,
            version = 1.ToString(),
        });

        await productActor.ProcessProductUpdate(new Product()
        {
            seller_id = 1,
            product_id = 1,
            price = 10,
            freight_value = 10,
            active = true,
            version = 2.ToString(),
        });

        var stockActor = _cluster.GrainFactory.GetGrain<ITransactionalStockActor>(1, 1.ToString());

        var version = (await stockActor.GetItem()).version;

        Assert.True(version.SequenceEqual("2"));
    }

}

