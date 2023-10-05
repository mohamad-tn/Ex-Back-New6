using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Bwr.Exchange.EntityFrameworkCore;
using Bwr.Exchange.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Bwr.Exchange.Web.Tests
{
    [DependsOn(
        typeof(ExchangeWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class ExchangeWebTestModule : AbpModule
    {
        public ExchangeWebTestModule(ExchangeEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ExchangeWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(ExchangeWebMvcModule).Assembly);
        }
    }
}