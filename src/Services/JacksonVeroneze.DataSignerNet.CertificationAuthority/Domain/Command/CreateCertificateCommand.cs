using MediatR;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Command
{
    public class CreateCertificateCommand : IRequest<CreateCertificateResult>
    {
        // O
        public string Organization { get; set; }

        // OU
        public string OrganizationalUnitName { get; set; }

        // T
        public string Title { get; set; }

        // CN
        public string CommonName { get; set; }

        // E
        public string EmailAddress { get; set; }

        // street
        public string Street { get; set; }

        // postalCode
        public string PostalCode { get; set; }

        // L - City
        public string LocalityName { get; set; }

        // ST
        public string StateOrProvinceName { get; set; }

        // C
        public string CountryCode { get; set; }

        // Surname
        public string Surname { get; set; }

        // GivenName
        public string GivenName { get; set; }

        // DateOfBirth
        public string DateOfBirth { get; set; }

        // Gender
        public string Gender { get; set; }

        public string Pin { get; set; }
    }
}
