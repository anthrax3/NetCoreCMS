﻿using System;
using System.Collections.Generic;
using System.Text;
using NetCoreCMS.Framework.Core.Repository;
using System.Linq;
using NetCoreCMS.Framework.Core.Models;
using NetCoreCMS.Framework.Core.Mvc.Models;

namespace NetCoreCMS.Framework.Core.Services
{
    public class NccWebSiteService : IBaseService<NccWebSite>
    {
        private readonly IBaseRepository<NccWebSite, long> _entityRepository;

        public NccWebSiteService(IBaseRepository<NccWebSite, long> entityRepository)
        {
            _entityRepository = entityRepository;
        }
         
        public NccWebSite Get(long entityId)
        {
            return _entityRepository.Query().FirstOrDefault(x => x.Id == entityId);
        }

        public NccWebSite Save(NccWebSite entity)
        {
            _entityRepository.Add(entity);
            _entityRepository.SaveChange();
            return entity;
        }

        public NccWebSite Update(NccWebSite entity)
        {
            var oldEntity = _entityRepository.Query().First(x => x.Id == entity.Id);
            CopyNewData(oldEntity, entity);
            _entityRepository.Eidt(oldEntity);
            _entityRepository.SaveChange();
            return entity;
        }
        
        public void Remove(long entityId)
        {
            var entity = _entityRepository.Query().FirstOrDefault(x => x.Id == entityId );
            if (entity != null)
            {
                entity.Status = EntityStatus.Deleted;
                _entityRepository.Eidt(entity);
                _entityRepository.SaveChange();
            }
        }

        public List<NccWebSite> GetAll()
        {
            return _entityRepository.Query().ToList();
        }

        public List<NccWebSite> GetAllByStatus(int status)
        {
            return _entityRepository.Query().Where(x => x.Status == status).ToList();
        }

        public List<NccWebSite> GetAllByName(string name)
        {
            return _entityRepository.Query().Where(x => x.Name == name).ToList();
        }

        public List<NccWebSite> GetAllByNameContains(string name)
        {
            return _entityRepository.Query().Where(x => x.Name.Contains(name)).ToList();
        }

        public void DeletePermanently(long entityId)
        {
            var entity = _entityRepository.Query().FirstOrDefault(x => x.Id == entityId);
            if (entity != null)
            {
                _entityRepository.Remove(entity);
                _entityRepository.SaveChange();
            }
        }

        private void CopyNewData(NccWebSite oldEntity, NccWebSite entity)
        {
            oldEntity.AllowRegistration = entity.AllowRegistration;
            oldEntity.Copyrights = entity.Copyrights;
            oldEntity.CreateBy = entity.CreateBy;
            oldEntity.CreationDate = entity.CreationDate;
            oldEntity.DateFormat = entity.DateFormat;
            oldEntity.DomainName = entity.DomainName;
            oldEntity.EmailAddress = entity.EmailAddress;
            oldEntity.FaviconUrl = entity.FaviconUrl;
            oldEntity.Language = entity.Language;
            oldEntity.ModificationDate = entity.ModificationDate;
            oldEntity.ModifyBy = entity.ModifyBy;
            oldEntity.Name = entity.Name;
            oldEntity.NewUserRole = entity.NewUserRole;
            oldEntity.SiteLogoUrl = entity.SiteLogoUrl;
            oldEntity.SiteTitle = entity.SiteTitle;
            oldEntity.Slug = entity.Slug;
            oldEntity.Status = entity.Status;
            oldEntity.Tagline = entity.Tagline;
            oldEntity.TimeFormat = entity.TimeFormat;
            oldEntity.TimeZone = entity.TimeZone;
            oldEntity.VersionNumber = entity.VersionNumber;
        }

        public string ToUniqueSlug(string slug, long entityId)
        {
            var i = 2;
            while (true)
            {
                var entity = _entityRepository.Query().FirstOrDefault(x => x.Slug == slug);
                if (entity != null && !(entity.Id == entityId))
                {
                    slug = string.Format("{0}-{1}", slug, i);
                    i++;
                }
                else
                {
                    break;
                }
            }

            return slug;
        }
    }
}