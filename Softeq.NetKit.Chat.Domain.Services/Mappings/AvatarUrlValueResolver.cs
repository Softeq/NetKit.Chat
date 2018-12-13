// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.Services.Mappings
{
    public class AvatarUrlValueResolver : IValueResolver<Member, object, string>
    {
        private readonly ICloudImageProvider _cloudImageProvider;

        public AvatarUrlValueResolver(ICloudImageProvider cloudImageProvider)
        {
            Ensure.That(cloudImageProvider).IsNotNull();

            _cloudImageProvider = cloudImageProvider;
        }

        string IValueResolver<Member, object, string>.Resolve(Member source, object destination, string destMember, ResolutionContext context)
        {
            return _cloudImageProvider.GetMemberAvatarUrl(source.PhotoName);
        }
    }
}