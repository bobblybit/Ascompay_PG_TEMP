namespace AscomPayPG.Models.DTO
{

    public class BlueSaltBvnVerificationResponseDTO
    {
        public int status_code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public Results results { get; set; }
    }

    public class Results
    {
        public string request_reference { get; set; }
        public string bvn_number { get; set; }
        public string name_on_card { get; set; }
        public object enrolment_branch { get; set; }
        public object enrolment_bank { get; set; }
        public object formatted_registration_date { get; set; }
        public string level_of_account { get; set; }
        public string nin { get; set; }
        public string watchlisted { get; set; }
        public string verification_status { get; set; }
        public string service_type { get; set; }
        public PersonalInfo personal_info { get; set; }
        public ResidentialInfo residential_info { get; set; }
    }


    public class PersonalInfo
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string phone_number { get; set; }
        public string phone_number_2 { get; set; }
        public string date_of_birth { get; set; }
        public string formatted_date_of_birth { get; set; }
        public string lga_of_origin { get; set; }
        public string state_of_origin { get; set; }
        public string nationality { get; set; }
        public string marital_status { get; set; }
    }
    public class ResidentialInfo
    {
        public string state_of_residence { get; set; }
        public string lga_of_residence { get; set; }
        public object residential_address { get; set; }
    }
}
