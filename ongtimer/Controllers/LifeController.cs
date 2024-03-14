using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ongtimer.Data;
using ongtimer.Models;

namespace ongtimer.Controllers
{
    [ApiController]
    [Route("Life")]
    public class LifeController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly int _totalLives = 5;

        public LifeController(DataContext context)
        {
            _context = context;
        }

     

        [HttpGet("GetLives")]
        public async Task<ActionResult<int>> GetLives()
        {
            var record = _context.UserRecords.FirstOrDefault();
            var pendingLives = record!.Count;
            var lastUpdateTimeUtc = record.LastUpdate;

            long secondsLeft = 0;
            long lives = 0;

            int remainingLives = _totalLives - pendingLives;

            if (remainingLives <= _totalLives)
            {
                long t = (long)(DateTime.UtcNow - lastUpdateTimeUtc).TotalSeconds;

                int lifeDuration = 60; 
                lives = t / lifeDuration;
                secondsLeft =(lifeDuration- (t % lifeDuration));

                if (lives>=1)
                {
                    record.LastUpdate = DateTime.UtcNow;
                    if(lives >= remainingLives)
                     lives = remainingLives;
                }
              
            }
       
            int totalLives = (int)(pendingLives + lives);

            if (totalLives == _totalLives)
                secondsLeft = 0;

            record.Count = totalLives;
    
            await _context.SaveChangesAsync();
            return Ok(new { RemainingLives = record.Count, RemainingSeconds= (int)secondsLeft });
        }




        [HttpGet("ConsumeLife")]
        public async Task<ActionResult<int>> ConsumeLife()
        {
            DateTime currentTimeUtc = DateTime.UtcNow;  // Use UTC time

            var record = _context.UserRecords.FirstOrDefault();
            int lives = record!.Count;

            if (lives >= 1)
            {
                if (lives == _totalLives)
                {
                     record.LastUpdate = currentTimeUtc;
                }
               record.Count = lives-1;
               
            }
       

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { RemainingLives = record.Count });
        }


        [HttpGet("ResetRecord")]
        public async Task<ActionResult<int>> Reset()
        {

            var record = _context.UserRecords.FirstOrDefault();
            DateTime currentTimeUtc = DateTime.UtcNow;

            if (record is null)
            {
                var newRecord = new UserRecord()
                {
                    Id = Guid.NewGuid(),
                    Count = _totalLives,
                    LastUpdate = currentTimeUtc
                };
                _context.UserRecords.Add(newRecord);
            }
            else
            {
                record.LastUpdate = currentTimeUtc;
                record.Count = _totalLives; ;
            }

          

            // Save changes to the database
            await _context.SaveChangesAsync();
            return Ok(new { ResetCount = _totalLives });
        }
    }

}
