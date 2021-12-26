using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using monday_integration.src.api;
using monday_integration.src.aqua;
using monday_integration.src.aqua.model;
using monday_integration.src.logging;
using monday_integration.src.monday;
using monday_integration.src.monday.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace monday_integration.src
{
    public static class Main
    {
        private static MondayIntegrationSettings settings;
        private static AimsLogger logger;

        public static async Task SyncMonday(ILogger logger) {
            Initialize(logger);

            try{
                await Execute();
            }catch(Exception) {
                throw;
            }finally{
                Cleanup();
            }
        }

        private static async Task Execute() {
            //var aquaClient = new AquaClient(settings.AimsJobId);
            //await aquaClient.RerunBackgroundJob();
            //var response = await aquaClient.FetchJsonData<WitreStylePO>();
            //logger.Info(response);

            var api = MondayApiFactory.GetApi();
            var options = new MondayBoardBodyOptions(){name=true};
            var boards = await api.GetMondayBoards(options);
            foreach(var board in boards) {
                logger.Info(JsonConvert.SerializeObject(board));
            }

            //foreach(var board in listOfBoards) {
            //    logger.Info(board);
            //}
        }



        private static void Initialize(ILogger logger) {
            AimsLoggerFactory.logger = logger;
            settings = new MondayIntegrationSettings(Environment.GetEnvironmentVariables());
            AimsApiFactory.InitializeApi(settings.Aims360BaseURL, settings.AimsBearerToken);
            MondayApiFactory.InitializeApi(settings.MondayBaseURL, settings.MondayApiKey);
            Main.logger = AimsLoggerFactory.CreateLogger(typeof(Main));
        }

        private static void Cleanup() {
            AimsApiFactory.CloseApi();
            MondayApiFactory.CloseApi();
        }
    }
}