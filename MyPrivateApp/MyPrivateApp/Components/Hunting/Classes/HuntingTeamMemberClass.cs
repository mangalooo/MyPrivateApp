
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingTeamMemberClass : IHuntingTeamMemberClass
    {
        private static HuntingTeamMembers? Get(ApplicationDbContext db, int? id) => db.HuntingTeamMembers.Any(r => r.HuntingTeamMembersId == id) ?
                                                                                db.HuntingTeamMembers.FirstOrDefault(r => r.HuntingTeamMembersId == id) :
                                                                                    throw new Exception("Medlemmen hittades inte i databasen!");

        public string Add(ApplicationDbContext db, HuntingTeamMembersViewModels vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (!string.IsNullOrEmpty(vm.Name) && !string.IsNullOrEmpty(vm.Mail) && !string.IsNullOrEmpty(vm.MobilePhone))
                {
                    try
                    {
                        HuntingTeamMembers model = ChangeFromViewModelToModel(vm);

                        db.HuntingTeamMembers.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till en ny medlem. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen name, e-post och mobil nummer ifulld!";

            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, HuntingTeamMembersViewModels vm)
        {
            if (vm != null && vm.HuntingTeamMembersId > 0 && db != null)
            {
                if (!string.IsNullOrEmpty(vm.Name) && !string.IsNullOrEmpty(vm.Mail) && !string.IsNullOrEmpty(vm.MobilePhone))
                {
                    try
                    {
                        HuntingTeamMembers getDbModel = Get(db, vm.HuntingTeamMembersId);

                        if (getDbModel != null)
                        {
                            getDbModel.HuntingTeamMembersId = vm.HuntingTeamMembersId;
                            getDbModel.Name = vm.Name;
                            getDbModel.Birthday = vm.Birthday.ToString("yyyy-MM-dd");
                            getDbModel.Address = vm.Address;
                            getDbModel.PostCode = vm.PostCode;
                            getDbModel.City = vm.City;
                            getDbModel.Mail = vm.Mail;
                            getDbModel.MobilePhone = vm.MobilePhone;
                            getDbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte medlemen i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra medlemmen. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen name, e-post och mobil nummer ifulld!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, HuntingTeamMembersViewModels vm, bool import)
        {
            if (vm != null && vm.HuntingTeamMembersId > 0 && db != null)
            {
                try
                {
                    HuntingTeamMembers model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.HuntingTeamMembers.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort medlemmen. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public HuntingTeamMembersViewModels ChangeFromModelToViewModel(HuntingTeamMembers model)
        {
            DateTime Birthday = DateTime.Parse(model.Birthday);

            HuntingTeamMembersViewModels vm = new()
            {
                HuntingTeamMembersId = model.HuntingTeamMembersId,
                Name = model.Name,
                Birthday = Birthday,
                Address = model.Address,
                PostCode = model.PostCode,
                City = model.City,
                Mail = model.Mail,
                MobilePhone = model.MobilePhone,
                Note = model.Note
            };

            return vm;
        }

        private static HuntingTeamMembers ChangeFromViewModelToModel(HuntingTeamMembersViewModels vm)
        {
            HuntingTeamMembers huntings = new()
            {
                HuntingTeamMembersId = vm.HuntingTeamMembersId,
                Name = vm.Name,
                Birthday = vm.Birthday.ToString("yyyy-MM-dd"),
                Address = vm.Address,
                PostCode = vm.PostCode,
                City = vm.City,
                Mail = vm.Mail,
                MobilePhone = vm.MobilePhone,
                Note = vm.Note
            };

            return huntings;
        }
    }
}