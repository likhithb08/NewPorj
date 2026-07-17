using LOCPS.Repositories.Interfaces;
using LOCPS.Models;
using LOCPS.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace LOCPS.Repositories.Implementation
{
    public class CreditEvaluationRepository : GenericRepository<CreditEvaluation>, ICreditEvaluationRepository
    {
        private readonly AppDbContext _context;
        public CreditEvaluationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CreditEvaluation> CreateCreditEvaluationAsync(CreditEvaluation CreditEvaluation)
        {
            await _context.CreditEvaluation.AddAsync(CreditEvaluation);
            await _context.SaveChangesAsync();
            return CreditEvaluation;
        }
        public async Task<CreditEvaluation?> GetCreditEvaluationByApplicationAsync(int applicationId)
        {
            return await _context.CreditEvaluation.FirstOrDefaultAsync(c => c.ApplicationId == applicationId);
        }

        public async Task<CreditEvaluation> UpdateCreditEvaluationAsync(CreditEvaluation CreditEvaluation)
        {
            _context.CreditEvaluation.Update(CreditEvaluation);
            await _context.SaveChangesAsync();
            return CreditEvaluation;
        }

        public async Task<IEnumerable<CreditEvaluation>> GetPendingCreditEvaluationAsync(CreditRecommendation CreditRecomendations)
        {
            return await _context.CreditEvaluation.Where(c => c.CreditRecomendations == CreditRecomendations).ToListAsync();
        }

        public async Task<CreditEvaluation?> GetEvaluationByofficerAsync(int EvaluatedByUserId)
        {
            return await _context.CreditEvaluation.FirstOrDefaultAsync(c => c.EvaluatedByUserId == EvaluatedByUserId);
        }
    }
}
