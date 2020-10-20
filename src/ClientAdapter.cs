using Atomus.Diagnostics;
using System;
using System.Threading.Tasks;

namespace Atomus.Service
{
    public class ClientAdapter : IServiceClient, IServiceClientAsync
    {
        private string Token;
        private readonly bool ByPassAll;
        private readonly string[] Authentication;

        public ClientAdapter()
        {
            string tmp;

            this.Token = "";

            try
            {
                this.ByPassAll = this.GetAttributeBool("ByPassAll");
            }
            catch (Exception ex)
            {
                this.ByPassAll = true;
                DiagnosticsTool.MyTrace(ex);
            }

            try
            {
                if (!this.ByPassAll)
                {
                    tmp = this.GetAttribute("Authentication");

                    this.Authentication = tmp.Split(',');
                }
            }
            catch (Exception ex)
            {
                this.Authentication = null;
                DiagnosticsTool.MyTrace(ex);
            }
        }

        Response IServiceClient.Request(IServiceDataSet serviceDataSet)
        {
            ICore core;
            //IService service;
            //IResponse response;

            try
            {
                //service = (IService)this.CreateInstance("Service");
                //core = (IService)Factory.CreateInstance(@"E:\Work\Project\Atomus\Service\WcfServiceClient\bin\Debug\Atomus.Service.WcfServiceClient.V1.0.0.0.dll", "Atomus.Service.WcfServiceClient", true, true);

                core = this.CreateInstance("Service");

                if (core is IService)
                    return this.RequestServiceDataSet(core, serviceDataSet);
                else
                    return this.RequestServiceData(core, serviceDataSet);
            }
            catch (AtomusException exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
        }

        Response RequestServiceDataSet(ICore core, IServiceDataSet serviceDataSet)
        {
            IResponse response;

            try
            {
                if (this.ByPassAll)//무조건 처리
                    return ((IService)core).Request((ServiceDataSet)serviceDataSet);
                else
                {
                    if (this.Authentication != null && this.Authentication.Length == 2
                        && serviceDataSet.DataTables.Contains(this.Authentication[0])
                        && serviceDataSet[this.Authentication[0]].CommandText.Contains(this.Authentication[1]))//로그인이 정적으로 처리 되면 토큰을 저장한다
                    {
                        response = ((IService)core).Request((ServiceDataSet)serviceDataSet);

                        if (response.Status == Status.OK)
                        {
                            this.Token = response.Message;
                            response.Message = null;
                        }

                        return (Response)response;
                    }

                    //토큰을 매번 보낸다
                    serviceDataSet["Token"].ConnectionName = "Atomus";
                    serviceDataSet["Token"].CommandText = this.Token;

                    return ((IService)core).Request((ServiceDataSet)serviceDataSet);
                }
            }
            catch (AtomusException exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
        }
        Response RequestServiceData(ICore core, IServiceDataSet serviceDataSet)
        {
            ServiceData serviceData;
            IResponse response;

            try
            {
                serviceData = new ServiceData();

                if (this.ByPassAll)//무조건 처리
                {
                    serviceData.ConvertServiceData(serviceDataSet);

                    return (Response)((IServiceExtensions)core).Request(serviceData).Response();
                }
                else
                {
                    if (this.Authentication != null && this.Authentication.Length == 2
                        && serviceDataSet.DataTables.Contains(this.Authentication[0])
                        && serviceDataSet[this.Authentication[0]].CommandText.Contains(this.Authentication[1]))//로그인이 정적으로 처리 되면 토큰을 저장한다
                    {
                        serviceData.ConvertServiceData(serviceDataSet);

                        response = ((IServiceExtensions)core).Request(serviceData).Response();

                        if (response.Status == Status.OK)
                        {
                            this.Token = response.Message;
                            response.Message = null;
                        }

                        return (Response)response;
                    }

                    //토큰을 매번 보낸다
                    serviceDataSet["Token"].ConnectionName = "Atomus";
                    serviceDataSet["Token"].CommandText = this.Token;

                    serviceData.ConvertServiceData(serviceDataSet);

                    return (Response)((IServiceExtensions)core).Request(serviceData).Response();
                }
            }
            catch (AtomusException exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
            //return (Response)_Response;
        }

        async Task<Response> IServiceClientAsync.RequestAsync(IServiceDataSet serviceDataSet)
        {
            ICore core;
            //IService _Service;

            try
            {
                //_Service = (IService)this.CreateInstance("Service");
                //_Service = (IServiceAsync)Factory.CreateInstance(@"E:\Work\Project\Atomus\Service\WcfServiceClient\bin\Debug\Atomus.Service.WcfServiceClient.V1.0.0.0.dll", "Atomus.Service.WcfServiceClient", true, true);

                core = this.CreateInstance("Service");

                if (core is IService)
                    return await RequestServiceDataSetAsync(core, serviceDataSet);
                else
                    return await RequestServiceDataAsync(core, serviceDataSet);
            }
            catch (AtomusException exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
        }

        async Task<Response> RequestServiceDataSetAsync(ICore core, IServiceDataSet serviceDataSet)
        {
            IResponse response;

            try
            {
                if (this.ByPassAll)//무조건 처리
                    return await ((dynamic)core).RequestAsync((ServiceDataSet)serviceDataSet);
                else
                {
                    if (this.Authentication != null && this.Authentication.Length == 2
                        && serviceDataSet.DataTables.Contains(this.Authentication[0])
                        && serviceDataSet[this.Authentication[0]].CommandText.Contains(this.Authentication[1]))//로그인이 정적으로 처리 되면 토큰을 저장한다
                    {
                        response = await ((dynamic)core).RequestAsync((ServiceDataSet)serviceDataSet);

                        if (response.Status == Status.OK)
                        {
                            this.Token = response.Message;
                            response.Message = null;
                        }

                        return (Response)response;
                    }

                    //토큰을 매번 보낸다
                    serviceDataSet["Token"].ConnectionName = "Atomus";
                    serviceDataSet["Token"].CommandText = this.Token;

                    return await ((dynamic)core).RequestAsync((ServiceDataSet)serviceDataSet);
                }
            }
            catch (AtomusException exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
        }
        async Task<Response> RequestServiceDataAsync(ICore core, IServiceDataSet serviceDataSet)
        {
            ServiceData serviceData;
            ServiceResult serviceResult;

            try
            {
                serviceData = new ServiceData();

                if (this.ByPassAll)//무조건 처리
                {
                    serviceData.ConvertServiceData(serviceDataSet);

                    serviceResult = await ((dynamic)core).RequestAsync(serviceData);

                    return (Response)serviceResult.Response();
                }
                else
                {
                    if (this.Authentication != null && this.Authentication.Length == 2
                        && serviceDataSet.DataTables.Contains(this.Authentication[0])
                        && serviceDataSet[this.Authentication[0]].CommandText.Contains(this.Authentication[1]))//로그인이 정적으로 처리 되면 토큰을 저장한다
                    {
                        serviceData.ConvertServiceData(serviceDataSet);

                        serviceResult = await ((dynamic)core).RequestAsync(serviceData);

                        if (serviceResult.Status == Status.OK)
                        {
                            this.Token = serviceResult.Message;
                            serviceResult.Message = null;
                        }

                        return (Response)serviceResult.Response();
                    }

                    //토큰을 매번 보낸다
                    serviceDataSet["Token"].ConnectionName = "Atomus";
                    serviceDataSet["Token"].CommandText = this.Token;

                    serviceData.ConvertServiceData(serviceDataSet);

                    serviceResult = await ((dynamic)core).RequestAsync(serviceData);

                    return (Response)serviceResult.Response();
                }
            }
            catch (AtomusException exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
                return (Response)Factory.CreateInstance("Atomus.Service.Response", false, true, exception);
            }
        }
    }
}
