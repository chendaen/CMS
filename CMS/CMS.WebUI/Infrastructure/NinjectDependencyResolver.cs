using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Moq;
using Ninject;
using CMS.Domain.Abstract;
using CMS.Domain.Entities;

namespace CMS.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            // put bindings here
            Mock<IUserRepository> mock = new Mock<IUserRepository>();
            mock.Setup(m=>m.Users).Returns(new List<User>{
                new User {  Name="test",Password="test"}
            });

            kernel.Bind<IUserRepository>().ToConstant(mock.Object);
        }
    }
}
}