namespace InstagramAvalon
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using InstagramApiSharp.API;
    using InstagramApiSharp.API.Builder;
    using InstagramApiSharp.Classes;
    using InstagramApiSharp.Classes.Models;
    using InstagramApiSharp.Logger;

    public static class InstagramUploader
    {
        const string stateFile = "state.bin";

        public static async Task<string> UploadVideo(InstagramData instagramData)
        {
            var userSession = new UserSessionData
            {

                UserName = instagramData.Username,
                Password = instagramData.Password
            };
            
            var delay = RequestDelay.FromSeconds(2, 2);

            var api = GetApi(delay, userSession);
            loadSessionData(api);
            var (successLogin, possibleMessage) = await LoginIfUnauthorised(api, delay);
            if (!successLogin) return possibleMessage;

            UpdateLoginState(api);

            var videoUpload = CreateNewVideoUpload(instagramData.VideoPath, instagramData.ImagePath);
            var result = await api.MediaProcessor.UploadVideoAsync(videoUpload, instagramData.Comments);

            return result.Succeeded
                ? $"Uploaded Successfully, Media created {result.Info.Message}"
                : $"Unable to upload video: {result.Info.Message}";
        }

        private static IInstaApi GetApi(IRequestDelay delay, UserSessionData userSessionData) =>
            InstaApiBuilder.CreateBuilder()
                .SetUser(userSessionData)
                .UseLogger(new DebugLogger(LogLevel.Exceptions)) // use logger for requests and debug messages
                .SetRequestDelay(delay)
                .Build();


        private static void loadSessionData(IInstaApi api)
        {
            try
            {
                if (!File.Exists(stateFile)) return;
                using var fs = File.OpenRead(stateFile);
                api.LoadStateDataFromStream(fs);
            }
            catch
            {
                //don't want to crash here
            }
        }

        private static async Task<(bool, string)> LoginIfUnauthorised(IInstaApi api, IRequestDelay delay)
        {
            delay.Disable();
            var logInResult = await api.LoginAsync();
            delay.Enable();
            return !logInResult.Succeeded 
                ? (false, $"unable to login {logInResult.Info.Message}") 
                : (true, String.Empty);
        }

        private static void UpdateLoginState(IInstaApi api)
        {
            var state = api.GetStateDataAsStream();
            try
            {
                using var fileStream = File.Create(stateFile);
                state.Seek(0, SeekOrigin.Begin);
                state.CopyTo(fileStream);
            }
            catch 
            {
                //don't want to crash here
            }
        }

        private static InstaVideoUpload CreateNewVideoUpload(string videoPath, string imagePath) =>
            new InstaVideoUpload
            {
                Video = new InstaVideo(videoPath, 0, 0),
                VideoThumbnail = new InstaImage(imagePath, 0, 0),
            };
    }
}