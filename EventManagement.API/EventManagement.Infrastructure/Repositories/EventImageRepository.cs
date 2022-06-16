using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Models.Dao;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Database;
using Microsoft.AspNetCore.Routing.Constraints;

namespace EventManagement.Infrastructure.Repositories
{
    public class EventImageRepository : BaseRepository, IEventImageRepository
    {
        public EventImageRepository(EventContext context) : base(context)
        {
        }


        public async Task AddNewImageAsync(EventImage eventImage, int eventId, IDbTransaction transaction = null)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);
            param.Add("@isMain", eventImage.IsMain);
            param.Add("@imagePath", eventImage.ImagePath);
            param.Add("@imageTitle", eventImage.ImageTitle);
            param.Add("@description", eventImage.Description.Description);

            var result = await this.ExecuteAsync("image_createNewImage_I", param,
                this.Transaction, commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException(ResponseStrings.SaveImageFailed);
            }
        }

        public async Task<EventImageDao> GetImageByIdAsync(int imageId)
        {
            var param = new DynamicParameters();

            param.Add("@imageId", imageId);

            var result = (await this.QueryAsync<EventImageDao>("image_getImageById_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (result == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result;

        }

        public async Task<List<EventImageDao>> GetImagesByEventIdAsync(int eventId)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);

            var result = (await this.QueryAsync<EventImageDao>("image_getByEventId_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure))?.ToList();

            if (result?.Any() == false)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result;
        }

        public async Task UpdateMainStatus(int imageId, bool isMain)
        {
            var param = new DynamicParameters();

            param.Add("@imageId", imageId);
            param.Add("@isMain", isMain);

            var result = await this.ExecuteAsync("image_updateMainStatus_I", param,
                this.Transaction, commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException(ResponseStrings.ImageUpdateFailed);
            }
        }

        public async Task RemoveImageAsync(int imageId)
        {
            var param = new DynamicParameters();

            param.Add("@imageId", imageId);

            var result = await this.ExecuteAsync("image_removeImage_D", param,
                this.Transaction, commandType: CommandType.StoredProcedure);

            if (result <= 0)
            {
                throw new DbException(ResponseStrings.OperationFailed);
            }
        }
    }
}