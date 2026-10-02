using System.Collections.Generic;
using CvWarehouse.Core.Cv.Dto;

namespace CvWarehouse.Core.Cv
{
    internal sealed class CvHeaderMapper
    {
        private readonly List<string> warnings;

        public CvHeaderMapper(List<string> warnings)
        {
            this.warnings = warnings;
        }

        public CvHeader Map(CvFileDto file)
        {
            string name = CvText.Clean(file.name);
            if (name.Length == 0)
                warnings.Add("The CV file has no name.");

            return new CvHeader
            {
                Name = name,
                Title = CvText.Clean(file.title),
                Contacts = MapContacts(file.contacts)
            };
        }

        private List<CvContact> MapContacts(CvContactDto[] contactDtos)
        {
            var contacts = new List<CvContact>();
            if (contactDtos == null)
                return contacts;

            for (int contactIndex = 0; contactIndex < contactDtos.Length; contactIndex++)
                AddContact(contacts, contactDtos[contactIndex], contactIndex);
            return contacts;
        }

        private void AddContact(List<CvContact> contacts, CvContactDto contactDto, int contactIndex)
        {
            string text = contactDto == null ? string.Empty : CvText.Clean(contactDto.text);
            if (text.Length == 0)
            {
                warnings.Add("Contact " + contactIndex + " has no text and was skipped.");
                return;
            }

            contacts.Add(new CvContact
            {
                Id = CvItemId.ForContact(contactIndex),
                Kind = CvText.Clean(contactDto.kind),
                Text = text,
                Link = CvText.Clean(contactDto.link)
            });
        }
    }
}
