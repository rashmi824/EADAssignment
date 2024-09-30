public interface IVendorService
{
    Task<Vendor> CreateVendor(string name);
    Task AddComment(string vendorId, string customerId, string commentText, double rating);
    Task<IReadOnlyList<Vendor>> GetVendors(); // Changed to IReadOnlyList for better encapsulation
}
