namespace models {
    public class Diagnosis {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? DiagnosisID { get; set; }
        public string DiagnosisDescription { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryDescription { get; set; }

        public string CategoryScore { get; set; }
        public bool MostSevereCategory { get; set; }

    }
}
