using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Repositories.UsersRepository;
using QMS.Core.Services.SystemLogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.VendorRepository
{
    public class VendorRepository : SqlTableRepository, IVendorRepository
    {
        private new readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public VendorRepository(QMSDbContext dbContext,
                               ISystemLogService systemLogService) : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<VendorViewModel>> GetListAsync()
        {
            try
            {
                var result = await _dbContext.Vendor_List.FromSqlRaw("EXEC sp_Get_Vendors").ToListAsync();

                // Map results to ViewModel
                var viewModelList = result.Select(data => new VendorViewModel
                {
                    Id = data.Id,
                    Vendor_Code = data.Vendor_Code,
                    Name = data.Name,
                    Email = data.Email,
                    Address = data.Address,
                    MobileNo = data.MobileNo,
                    Contact_Persons = data.Contact_Persons,
                    Owner = data.Owner,
                    Plant_Head = data.Plant_Head,
                    Quality_Manager = data.Quality_Manager,
                    PDG_Manager = data.PDG_Manager,
                    SCM_Manager = data.SCM_Manager,
                    PRD_Manager = data.PRD_Manager,
                    Service_Manager = data.Service_Manager,
                    User_Name = data.User_Name,
                    CreatedBy = data.CreatedBy,
                    CreatedDate = data.CreatedDate,
                    UpdatedBy = data.UpdatedBy,
                    UpdatedDate = data.UpdatedDate,
                }).ToList();

                return viewModelList;

            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
        public async Task<CertificationDetail> CertGetByIdAsync(int id)
        {
            return await _dbContext.CertificationDetails
                .FirstOrDefaultAsync(x => x.Id == id && x.Deleted == false);
        }

        public async Task<OperationResult> CreateAsync(Vendor newRecord, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Vendor_Code", newRecord.Vendor_Code ?? (object)DBNull.Value),
                    new SqlParameter("@Name", newRecord.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Address", newRecord.Address ?? (object)DBNull.Value),
                    new SqlParameter("@Email", newRecord.Email ?? (object)DBNull.Value),
                    new SqlParameter("@MobileNo", newRecord.MobileNo ?? (object)DBNull.Value),
                    new SqlParameter("@GstNo", newRecord.GstNo ?? (object)DBNull.Value),
                    new SqlParameter("@Contact_Persons", newRecord.Contact_Persons ?? (object)DBNull.Value),
                    new SqlParameter("@Owner", newRecord.Owner ?? (object)DBNull.Value),
                    new SqlParameter("@Owner_Email", newRecord.Owner_Email ?? (object)DBNull.Value),
                    new SqlParameter("@Owner_Mobile", newRecord.Owner_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Plant_Head", newRecord.Plant_Head ?? (object)DBNull.Value),
                    new SqlParameter("@Plant_Email", newRecord.Plant_Email ?? (object)DBNull.Value),
                    new SqlParameter("@Plant_Mobile", newRecord.Plant_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Quality_Manager", newRecord.Quality_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@Quality_Email", newRecord.Quality_Email ?? (object)DBNull.Value),
                    new SqlParameter("@Quality_Mobile", newRecord.Quality_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@PDG_Manager", newRecord.PDG_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@PDG_Email", newRecord.PDG_Email ?? (object)DBNull.Value),
                    new SqlParameter("@PDG_Mobile", newRecord.PDG_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@SCM_Manager", newRecord.SCM_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@SCM_Email", newRecord.SCM_Email ?? (object)DBNull.Value),
                    new SqlParameter("@SCM_Mobile", newRecord.SCM_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@PRD_Manager", newRecord.PRD_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@PRD_Email", newRecord.PRD_Email ?? (object)DBNull.Value),
                    new SqlParameter("@PRD_Mobile", newRecord.PRD_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Service_Manager", newRecord.Service_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@Service_Email", newRecord.Service_Email ?? (object)DBNull.Value),
                    new SqlParameter("@Service_Mobile", newRecord.Service_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Other_One", newRecord.Other_Cont_One ?? (object)DBNull.Value),
                    new SqlParameter("@Other_OneEmail", newRecord.Other_Cont_OneEmail ?? (object)DBNull.Value),
                    new SqlParameter("@Other_OneMobile", newRecord.Other_Cont_OneMobile ?? (object)DBNull.Value),
                    new SqlParameter("@Other_Two", newRecord.Other_Cont_Two ?? (object)DBNull.Value),
                    new SqlParameter("@Other_TwoEmail", newRecord.Other_Cont_TwoEmail ?? (object)DBNull.Value),
                    new SqlParameter("@Other_TwoMobile", newRecord.Other_Cont_TwoMobile ?? (object)DBNull.Value),
                    new SqlParameter("@User_Name", newRecord.User_Name ?? (object)DBNull.Value),
                    new SqlParameter("@Password", newRecord.Password ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", newRecord.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedDate", newRecord.CreatedDate ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", newRecord.Deleted),
                };

                var sql = @"EXEC sp_Insert_Vendor @Vendor_Code,@Name,@Address,@Email,@MobileNo,@GstNo,@Contact_Persons,@Owner,@Owner_Email,@Owner_Mobile,@Plant_Head,@Plant_Email,@Plant_Mobile,
                        @Quality_Manager,@Quality_Email,@Quality_Mobile,@PDG_Manager,@PDG_Email,@PDG_Mobile,@SCM_Manager,@SCM_Email,@SCM_Mobile,@PRD_Manager,@PRD_Email,@PRD_Mobile,
                        @Service_Manager,@Service_Email,@Service_Mobile,@Other_One,@Other_OneEmail,@Other_OneMobile,@Other_Two,@Other_TwoEmail,@Other_TwoMobile,@User_Name,@Password,@CreatedBy,@CreatedDate,@IsDeleted";

                await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

                if (returnCreatedRecord)
                {
                    var user = new Users
                    {
                        Name = newRecord.Name,
                        Email = newRecord.Email,
                        Username = newRecord.User_Name,
                        Password = newRecord.Password,
                        MobileNo = newRecord.MobileNo,
                        User_Type = "Vendor"
                    };

                    await _dbContext.User.AddAsync(user); // Correct DbSet name assumed to be 'Users'
                    await _dbContext.SaveChangesAsync();

                    return new OperationResult
                    {
                        Success = true,
                    };
                }

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> UpdateAsync(Vendor updatedRecord, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Ven_Id", updatedRecord.Id),
                    new SqlParameter("@Vendor_Code", updatedRecord.Vendor_Code ?? (object)DBNull.Value),
                    new SqlParameter("@Name", updatedRecord.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Address", updatedRecord.Address ?? (object)DBNull.Value),
                    new SqlParameter("@Email", updatedRecord.Email ?? (object)DBNull.Value),
                    new SqlParameter("@MobileNo", updatedRecord.MobileNo ?? (object)DBNull.Value),
                    new SqlParameter("@GstNo", updatedRecord.GstNo ?? (object)DBNull.Value),
                    new SqlParameter("@Contact_Persons", updatedRecord.Contact_Persons ?? (object)DBNull.Value),
                    new SqlParameter("@Owner", updatedRecord.Owner ?? (object)DBNull.Value),
                    new SqlParameter("@Owner_Email", updatedRecord.Owner_Email ?? (object)DBNull.Value),
                    new SqlParameter("@Owner_Mobile", updatedRecord.Owner_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Plant_Head", updatedRecord.Plant_Head ?? (object)DBNull.Value),
                    new SqlParameter("@Plant_Email", updatedRecord.Plant_Email ?? (object)DBNull.Value),
                    new SqlParameter("@Plant_Mobile", updatedRecord.Plant_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Quality_Manager", updatedRecord.Quality_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@Quality_Email", updatedRecord.Quality_Email ?? (object)DBNull.Value),
                    new SqlParameter("@Quality_Mobile", updatedRecord.Quality_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@PDG_Manager", updatedRecord.PDG_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@PDG_Email", updatedRecord.PDG_Email ?? (object)DBNull.Value),
                    new SqlParameter("@PDG_Mobile", updatedRecord.PDG_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@SCM_Manager", updatedRecord.SCM_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@SCM_Email", updatedRecord.SCM_Email ?? (object)DBNull.Value),
                    new SqlParameter("@SCM_Mobile", updatedRecord.SCM_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@PRD_Manager", updatedRecord.PRD_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@PRD_Email", updatedRecord.PRD_Email ?? (object)DBNull.Value),
                    new SqlParameter("@PRD_Mobile", updatedRecord.PRD_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Service_Manager", updatedRecord.Service_Manager ?? (object)DBNull.Value),
                    new SqlParameter("@Service_Email", updatedRecord.Service_Email ?? (object)DBNull.Value),
                    new SqlParameter("@Service_Mobile", updatedRecord.Service_Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Other_One", updatedRecord.Other_Cont_One ?? (object)DBNull.Value),
                    new SqlParameter("@Other_OneEmail", updatedRecord.Other_Cont_OneEmail ?? (object)DBNull.Value),
                    new SqlParameter("@Other_OneMobile", updatedRecord.Other_Cont_OneMobile ?? (object)DBNull.Value),
                    new SqlParameter("@Other_Two", updatedRecord.Other_Cont_Two ?? (object)DBNull.Value),
                    new SqlParameter("@Other_TwoEmail", updatedRecord.Other_Cont_TwoEmail ?? (object)DBNull.Value),
                    new SqlParameter("@Other_TwoMobile", updatedRecord.Other_Cont_TwoMobile ?? (object)DBNull.Value),
                    new SqlParameter("@User_Name", updatedRecord.User_Name ?? (object)DBNull.Value),
                    new SqlParameter("@Password", updatedRecord.Password ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", updatedRecord.UpdatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedDate", updatedRecord.UpdatedDate ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", updatedRecord.Deleted),
                };

                var sql = @"EXEC sp_Update_Vendor @Ven_Id,@Vendor_Code,@Name,@Address,@Email,@MobileNo,@GstNo,@Contact_Persons,@Owner,@Owner_Email,@Owner_Mobile,@Plant_Head,@Plant_Email,@Plant_Mobile,
                        @Quality_Manager,@Quality_Email,@Quality_Mobile,@PDG_Manager,@PDG_Email,@PDG_Mobile,@SCM_Manager,@SCM_Email,@SCM_Mobile,@PRD_Manager,@PRD_Email,@PRD_Mobile,
                        @Service_Manager,@Service_Email,@Service_Mobile,@Other_One,@Other_OneEmail,@Other_OneMobile,@Other_Two,@Other_TwoEmail,@Other_TwoMobile,@User_Name,@Password,@UpdatedBy,@UpdatedDate,@IsDeleted";

                await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

                if (returnUpdatedRecord)
                {
                    return new OperationResult
                    {
                        Success = true,
                    };
                }

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> DeleteAsync(int userId)
        {
            try
            {
                return await base.DeleteAsync<Vendor>(userId);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<VendorViewModel?> GetByIdAsync(int ven_Id)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Ven_Id", ven_Id),
                };

                var sql = @"EXEC sp_GetVendor_By_Id @Ven_Id";

                var result = await _dbContext.Vendor.FromSqlRaw(sql, parameters).ToListAsync();

                var viewModelList = result.Select(data => new VendorViewModel
                {
                    Id = data.Id,
                    Vendor_Code = data.Vendor_Code,
                    Name = data.Name,
                    Email = data.Email,
                    Address = data.Address,
                    MobileNo = data.MobileNo,
                    GstNo = data.GstNo,
                    Contact_Persons = data.Contact_Persons,
                    Owner = data.Owner,
                    Owner_Email = data.Owner_Email,
                    Owner_Mobile = data.Owner_Mobile,
                    Plant_Head = data.Plant_Head,
                    Plant_Email = data.Plant_Email,
                    Plant_Mobile = data.Plant_Mobile,
                    Quality_Manager = data.Quality_Manager,
                    Quality_Email = data.Quality_Email,
                    Quality_Mobile = data.Quality_Mobile,
                    PDG_Manager = data.PDG_Manager,
                    PDG_Email = data.PDG_Email,
                    PDG_Mobile = data.PDG_Mobile,
                    SCM_Manager = data.SCM_Manager,
                    SCM_Email = data.SCM_Email,
                    SCM_Mobile = data.SCM_Mobile,
                    PRD_Manager = data.PRD_Manager,
                    PRD_Email = data.PRD_Email,
                    PRD_Mobile = data.PRD_Mobile,
                    Service_Manager = data.Service_Manager,
                    Service_Email = data.Service_Manager,
                    Service_Mobile = data.Service_Mobile,
                    Other_Cont_One = data.Other_Cont_One,
                    Other_Cont_OneEmail = data.Other_Cont_OneEmail,
                    Other_Cont_OneMobile = data.Other_Cont_OneMobile,
                    Other_Cont_Two = data.Other_Cont_Two,
                    Other_Cont_TwoEmail = data.Other_Cont_TwoEmail,
                    Other_Cont_TwoMobile = data.Other_Cont_TwoMobile,
                    User_Name = data.User_Name,
                    CreatedBy = data.CreatedBy,
                    CreatedDate = data.CreatedDate
                }).ToList();

                return viewModelList.FirstOrDefault(); // Assuming you want a single view model based on the ID

            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<bool> CheckDuplicate(string searchText, int Id)
        {
            try
            {
                bool existingflag = false;
                int? existingId = null;

                IQueryable<int> query = _dbContext.Vendor
                    .Where(x => x.Deleted == false && x.Name.ToString() == searchText)
                    .Select(x => x.Id);

                // Add additional condition if Id is not 0
                if (Id != 0)
                {
                    query = _dbContext.Vendor
                        .Where(x => x.Deleted == false &&
                               x.Name.ToString() == searchText
                               && x.Id != Id)
                        .Select(x => x.Id);
                }


                existingId = await query.FirstOrDefaultAsync();

                if (existingId != null && existingId > 0)
                {
                    existingflag = true;
                }

                return existingflag;
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
        //Certification Details
        public async Task<List<CertificationDetailViewModel>> CertGetAllAsync()
        {
            try
            {
                var result = await _dbContext.CertificationDetailViewModel
                    .FromSqlRaw("EXEC dbo.GetAllCertificationDetails")
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }



        public async Task<OperationResult> CertCreateAsync(CertificationDetail cert)
        {
            cert.CreatedDate = DateTime.Now;
            _dbContext.CertificationDetails.Add(cert);
            await _dbContext.SaveChangesAsync();

            return new OperationResult { Success = true, ObjectId = cert.Id };
        }

        public async Task<OperationResult> CertUpdateAsync(CertificationDetail cert)
        {
            var existing = await _dbContext.CertificationDetails.FindAsync(cert.Id);
            if (existing == null)
                return new OperationResult { Success = false, Message = "Record not found" };

            existing.ProductCode = cert.ProductCode;
            existing.VendorID = cert.VendorID;
            existing.IssueDate = cert.IssueDate;
            existing.ExpiryDate = cert.ExpiryDate;
            existing.CertUpload = cert.CertUpload;
            existing.Remarks = cert.Remarks;
            existing.UpdatedDate = DateTime.Now;
            existing.UpdatedBy = cert.UpdatedBy;
            existing.VendorCode = cert.VendorCode;
            existing.CertificateMasterId = cert.CertificateMasterId;

            await _dbContext.SaveChangesAsync();
            return new OperationResult { Success = true };
        }

        public async Task<OperationResult> CertDeleteAsync(int id, string updatedBy)
        {
            var existing = await _dbContext.CertificationDetails.FindAsync(id);
            if (existing == null)
                return new OperationResult { Success = false, Message = "Record not found" };

            existing.UpdatedBy = updatedBy;
            existing.UpdatedDate = DateTime.Now;
            //  existing.Deleted = true;
            await _dbContext.SaveChangesAsync();
            return new OperationResult { Success = true };
        }

        //Third Party Testing Report//
        public async Task<List<ThirdPartyTestReportViewModel>> ReportGetAllAsync()
        {
            try
            {
                var result = await _dbContext.ThirdPartyTestReportViewModels
                    .FromSqlRaw("EXEC dbo.GetAllReportDetails")
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> ReportCreateAsync(ThirdPartyTestReport input)
        {
            input.CreatedDate = DateTime.Now;
            _dbContext.ThirdPartyTestReports.Add(input);
            await _dbContext.SaveChangesAsync();

            return new OperationResult { Success = true, ObjectId = input.Id };
        }

        public async Task<OperationResult> ReportUpdateAsync(ThirdPartyTestReport input)
        {
            var existing = await _dbContext.ThirdPartyTestReports.FindAsync(input.Id);
            if (existing == null)
                return new OperationResult { Success = false, Message = "Record not found" };

            existing.ProductCode = input.ProductCode;
            existing.VendorID = input.VendorID;
            existing.IssueDate = input.IssueDate;
            existing.ExpiryDate = input.ExpiryDate;
            existing.ReportFileName = input.ReportFileName;
            existing.Remarks = input.Remarks;
            existing.UpdatedDate = DateTime.Now;
            existing.UpdatedBy = input.UpdatedBy;
            existing.VendorCode = input.VendorCode;
            existing.ThirdPartyReportID = input.ThirdPartyReportID;

            await _dbContext.SaveChangesAsync();
            return new OperationResult { Success = true };
        }

        public async Task<OperationResult> ReportDeleteAsync(int id, string updatedBy)
        {
            var existing = await _dbContext.ThirdPartyTestReports.FindAsync(id);
            if (existing == null)
                return new OperationResult { Success = false, Message = "Record not found" };
            existing.Deleted = true;
            existing.UpdatedBy = updatedBy;
            existing.UpdatedDate = DateTime.Now;
            //  existing.Deleted = true;
            await _dbContext.SaveChangesAsync();
            return new OperationResult { Success = true };
        }
        public async Task<ThirdPartyTestReport> ReportGetByIdAsync(int id)
        {
            var entity = await _dbContext.ThirdPartyTestReports
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new ThirdPartyTestReport
                {
                    Id = x.Id,
                    VendorID = x.VendorID,
                    VendorCode = x.VendorCode,
                    ThirdPartyReportID = x.ThirdPartyReportID,
                    ProductCode = x.ProductCode,
                    IssueDate = x.IssueDate,
                    ExpiryDate = x.ExpiryDate,
                    Remarks = x.Remarks,
                    ReportFileName = x.ReportFileName
                })
                .FirstOrDefaultAsync();

            return entity;
        }

        ////public class ProductCodeDetailDto
        ////{
        ////    public string OldPart_No { get; set; }
        ////    public string Description { get; set; }
        ////}
        //public async Task<List<ProductCodeDetailDto>> GetProductCodesAsync(string term)
        //{
        //    var results = new List<ProductCodeDetailViewModel>();
        //    // Get the raw ADO.NET connection:
        //    var conn = _dbContext.Database.GetDbConnection();
        //    await conn.OpenAsync();

        //    using (var cmd = conn.CreateCommand())
        //    {
        //        cmd.CommandText = "dbo.GetProductCodeDetails";
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        var p = cmd.CreateParameter();
        //        p.ParameterName = "@Term";
        //        p.Value = (object)term ?? DBNull.Value;
        //        cmd.Parameters.Add(p);

        //        using (var reader = await cmd.ExecuteReaderAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                results.Add(new ProductCodeDetailViewModel
        //                {
        //                    OldPart_No = reader.GetString(0),
        //                    Description = reader.GetString(1),
        //                });
        //            }
        //        }
        //    }

        //    return results;
        //}

        public async Task<List<ProductCodeDetailViewModel>> GetCodeSearchAsync(string search = "")
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@OldPartNo", search),
                };

                var sql = @"EXEC sp_GetProductCode_Detail_ByCode @OldPartNo";

                var result = await _dbContext.ProductCode.FromSqlRaw(sql, parameters).ToListAsync();

                var viewModelList = result.Select(data => new ProductCodeDetailViewModel
                {
                    PCDetails_Id = data.PCDetails_Id,
                    OldPart_No = data.OldPart_No,
                    Description = data.Description
                }).ToList();

                return viewModelList; // Assuming you want a single view model based on the ID
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync()
        {
            return await _dbContext.Vendor
                .Where(v => !v.Deleted)
                .Select(v => new DropdownOptionViewModel
                {
                    Label = v.Name,
                    Value = v.Vendor_Code
                })
                .Distinct() 
                .ToListAsync();
        }
        public async Task<List<VenBISCertificateViewModel>> GetAllBISCertificatesAsync()
        {
            return await _dbContext.VenBISCertificates
                .Where(c => c.Deleted != true)
                .Select(c => new VenBISCertificateViewModel
                {
                    ID = c.Id,
                    ProductCode = c.ProductCode,
                    VendorID = c.VendorID,
                    VendorCode = c.VendorCode,
                    BISSection = c.BISSection,
                    RNumber = c.RNumber,
                    ModelNo = c.ModelNo,
                    Remarks = c.Remarks,
                    IssueDate = c.IssueDate,
                    ExpiryDate = c.ExpiryDate,
                    FileName = c.FileName,
                    CreatedBy = c.CreatedBy,
                    CreatedDate = c.CreatedDate,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedDate = c.UpdatedDate,
                    CertificateDetail = c.CertificateDetail
                })
                .ToListAsync();
        }

        public async Task<VenBISCertificate?> GetBISCertificateByIdAsync(int id)
        {
            return await _dbContext.VenBISCertificates
                .FirstOrDefaultAsync(c => c.Id == id && c.Deleted != true);
        }
     

        public async Task<bool> CreateOrUpdateBISCertificateAsync(VenBISCertificate cert)
        {
            if (cert.Id == 0)
            {
                cert.CreatedDate = DateTime.Now;
                await _dbContext.VenBISCertificates.AddAsync(cert);
            }
            else
            {
                cert.UpdatedDate = DateTime.Now;
                _dbContext.VenBISCertificates.Update(cert);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBISCertificateAsync(int id, string updatedBy)
        {
            var cert = await _dbContext.VenBISCertificates.FindAsync(id);
            if (cert == null)
                return false;

            cert.Deleted = true;
            cert.UpdatedBy = updatedBy;
            cert.UpdatedDate = DateTime.Now;

            _dbContext.VenBISCertificates.Update(cert);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}