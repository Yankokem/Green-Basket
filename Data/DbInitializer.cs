using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Green_Basket.Models;

namespace Green_Basket.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Ensure database schema is created
            await context.Database.EnsureCreatedAsync();

            // Ensure FullName column exists if table was created previously without it
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF COL_LENGTH('AspNetUsers', 'FullName') IS NULL 
                    BEGIN
                        ALTER TABLE AspNetUsers ADD FullName nvarchar(100) NOT NULL DEFAULT '';
                    END
                ");
            }
            catch
            {
                // Ignore if already handled by EF Core
            }

            // 1. Seed Roles
            string[] roles = new[] { "Admin", "DRCoordinator", "Staff" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Named Accounts (Plain names only, no ranks or titles)
            var usersToSeed = new[]
            {
                new { FullName = "Alexander Hayes", Email = "admin@greenbasket.local", Role = "Admin" },
                new { FullName = "Marcus Vance", Email = "coordinator@greenbasket.local", Role = "DRCoordinator" },
                new { FullName = "Clara Oswald", Email = "staff@greenbasket.local", Role = "Staff" }
            };

            foreach (var u in usersToSeed)
            {
                var existingUser = await userManager.FindByEmailAsync(u.Email);
                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FullName = u.FullName,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, "123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, u.Role);
                    }
                }
                else
                {
                    // Update full name if empty or missing
                    if (string.IsNullOrEmpty(existingUser.FullName) || existingUser.FullName != u.FullName)
                    {
                        existingUser.FullName = u.FullName;
                        await userManager.UpdateAsync(existingUser);
                    }
                }
            }

            // 3. Seed Initial Hospital IT Assets if table is empty
            if (!context.Assets.Any())
            {
                var emrAsset = new Asset
                {
                    Name = "Epic Electronic Medical Records (EMR)",
                    Type = "Clinical Core",
                    Owner = "Sarah Chen",
                    Priority = "Critical",
                    RtoMinutes = 60,
                    RpoMinutes = 15,
                    BackupDependency = "PureStorage SAN Snapshot + Azure DR Warm Standby"
                };

                var pacsAsset = new Asset
                {
                    Name = "Radiology PACS Server",
                    Type = "Imaging & Diagnostics",
                    Owner = "Mark Henderson",
                    Priority = "Critical",
                    RtoMinutes = 120,
                    RpoMinutes = 30,
                    BackupDependency = "Local Veeam Repository + Offsite Tape Archive"
                };

                var pharmacyAsset = new Asset
                {
                    Name = "In-Patient Automated Pharmacy Dispenser",
                    Type = "Medication Dispensing",
                    Owner = "Elena Rostova",
                    Priority = "High",
                    RtoMinutes = 180,
                    RpoMinutes = 60,
                    BackupDependency = "SQL Server AlwaysOn Availability Group replica"
                };

                var billingAsset = new Asset
                {
                    Name = "Hospital Billing & Insurance Gateway",
                    Type = "Financial / Administrative",
                    Owner = "David Miller",
                    Priority = "Medium",
                    RtoMinutes = 480,
                    RpoMinutes = 120,
                    BackupDependency = "Daily Veeam Backup to Secondary Datacenter"
                };

                context.Assets.AddRange(emrAsset, pacsAsset, pharmacyAsset, billingAsset);
                await context.SaveChangesAsync();

                // 4. Seed Procedures
                context.RecoveryProcedures.AddRange(
                    new RecoveryProcedure
                    {
                        AssetId = emrAsset.Id,
                        Title = "Ransomware Containment & Standby Failover",
                        Steps = "1. Sever primary clinical VLAN uplink immediately.\n2. Isolate infected database nodes via Cisco ISE.\n3. Verify integrity of SAN immutable snapshot replica.\n4. Trigger Azure warm standby failover sequence.\n5. Redirect hospital DNS to DR endpoint."
                    },
                    new RecoveryProcedure
                    {
                        AssetId = pacsAsset.Id,
                        Title = "PACS DICOM Store Restoration",
                        Steps = "1. Boot secondary PACS VM image from disaster recovery vault.\n2. Mount sanitized image volume.\n3. Re-index last known clean DICOM headers.\n4. Run modality handshake test from CT and MRI suites."
                    }
                );

                // 5. Seed Contacts (Plain names, no ranks)
                context.Contacts.AddRange(
                    new Contact
                    {
                        AssetId = emrAsset.Id,
                        Name = "James Alvarez",
                        Role = "Lead Database Administrator",
                        Phone = "+1 (555) 019-2834",
                        Email = "jalvarez@greenbasket-hospital.org"
                    },
                    new Contact
                    {
                        AssetId = emrAsset.Id,
                        Name = "Sarah Chen",
                        Role = "Chief Medical Information Officer",
                        Phone = "+1 (555) 019-8721",
                        Email = "schen@greenbasket-hospital.org"
                    },
                    new Contact
                    {
                        AssetId = pacsAsset.Id,
                        Name = "Mark Henderson",
                        Role = "Senior Imaging Engineer",
                        Phone = "+1 (555) 019-5412",
                        Email = "mhenderson@greenbasket-hospital.org"
                    }
                );

                // 6. Seed Recovery Tests
                context.RecoveryTests.AddRange(
                    new RecoveryTest
                    {
                        AssetId = emrAsset.Id,
                        TestDate = DateTime.Today.AddDays(-14),
                        Result = "Passed",
                        ActualRtoMinutes = 45,
                        ActualRpoMinutes = 10,
                        Notes = "Simulated SAN controller failure. Warm standby booted successfully without schema drift.",
                        LessonsLearned = "DNS propagation took 8 minutes; consider shortening TTL during planned DR drills."
                    },
                    new RecoveryTest
                    {
                        AssetId = pacsAsset.Id,
                        TestDate = DateTime.Today.AddDays(-30),
                        Result = "Failed",
                        ActualRtoMinutes = 160,
                        ActualRpoMinutes = 45,
                        Notes = "DICOM index rebuild took longer than expected due to network throttle on backup NAS.",
                        LessonsLearned = "Need dedicated 10Gbps link between staging NAS and radiology imaging cluster."
                    }
                );

                // 7. Seed Recovery Tasks
                context.RecoveryTasks.AddRange(
                    new RecoveryTask
                    {
                        AssetId = emrAsset.Id,
                        Title = "Update SSL certificates on DR failover VIP",
                        AssignedTo = "James Alvarez",
                        Status = "In Progress",
                        DueDate = DateTime.Today.AddDays(7)
                    },
                    new RecoveryTask
                    {
                        AssetId = pacsAsset.Id,
                        Title = "Provision 10Gbps backup link to radiology cluster",
                        AssignedTo = "Mark Henderson",
                        Status = "Pending",
                        DueDate = DateTime.Today.AddDays(14)
                    },
                    new RecoveryTask
                    {
                        AssetId = pharmacyAsset.Id,
                        Title = "Validate automated medication cabinet offline sync",
                        AssignedTo = "Elena Rostova",
                        Status = "Done",
                        DueDate = DateTime.Today.AddDays(-3)
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
