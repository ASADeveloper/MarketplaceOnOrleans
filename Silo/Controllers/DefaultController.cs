﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using Orleans.Infra;
using Orleans.Interfaces;
using Orleans.Runtime;

namespace Silo.Controllers;

[ApiController]
public class DefaultController : ControllerBase
{
    private readonly IPersistence persistence;
    private readonly ILogger<DefaultController> logger;

    public DefaultController(IPersistence persistence, ILogger<DefaultController> logger)
    {
        this.persistence = persistence;
        this.logger = logger;
    }

    [Route("/reset")]
    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    public async Task<ActionResult> Reset([FromServices] IGrainFactory grains)
    {
        logger.LogWarning("Reset requested at {0}", DateTime.UtcNow);

        // SimpleGrainStatistic
        var mgmt = grains.GetGrain<IManagementGrain>(0);
        var stats = await mgmt.GetSimpleGrainStatistics();

        // get sellers and orders actors to reset
        // cannot get orders and sellers from shipments
        // because some of them may have been already removed from memory
        foreach(var stat in stats)
        {
            logger.LogDebug("{stat}",stat.ToString());
            if (stat.GrainType.SequenceEqual("Orleans.Grains.OrderActor,Orleans"))
            {
                int num = stat.ActivationCount;
                var tasks = new List<Task>();
                for(int i = 1; i <= num; i++)
                {
                    tasks.Add( grains.GetGrain<IOrderActor>(i).Reset() );
                }
                await Task.WhenAll(tasks);
                logger.LogWarning("{0} order states resetted", num);
                continue;
            }
            if (stat.GrainType.SequenceEqual("Orleans.Grains.SellerActor,Orleans"))
            {
                int num = stat.ActivationCount;
                var tasks = new List<Task>();
                for(int i = 1; i <= num; i++)
                {
                    tasks.Add( grains.GetGrain<ISellerActor>(i).Reset() );
                }
                await Task.WhenAll(tasks);
                logger.LogWarning("{0} seller states resetted", num);
                continue;
            }
            // seal carts that have not checked out in past run
            if (stat.GrainType.SequenceEqual("Orleans.Grains.CartActor,Orleans"))
            {
                int num = stat.ActivationCount;
                var tasks = new List<Task>();
                for(int i = 1; i <= num; i++)
                {
                    tasks.Add( grains.GetGrain<ICartActor>(i).Seal() );
                }
                await Task.WhenAll(tasks);
                logger.LogWarning("{0} cart states resetted", num);
            }
        }

        // Helper.CleanLog();
        await persistence.CleanLog();
        await ResetShipmentActors(grains);

        return Ok();
    }

    private static async Task ResetShipmentActors(IGrainFactory grains)
    {
        List<Task> tasks = new List<Task>(Constants.NumShipmentActors);
        for(int i = 0; i < Constants.NumShipmentActors; i++)
        {
            var grain = grains.GetGrain<IShipmentActor>(i);
            tasks.Add(grain.Reset());
        }
        await Task.WhenAll(tasks);
    }

    // should be called before shutting off the app server
    [Route("/cleanup")]
    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    public async Task<ActionResult> Cleanup()
    {
        this.logger.LogWarning("Cleanup requested at {0}", DateTime.UtcNow);
        // Helper.TruncateOrleansStorage();
        await persistence.TruncateStorage();
        // Helper.CleanLog();
        await persistence.CleanLog();
        return Ok();
    }

}