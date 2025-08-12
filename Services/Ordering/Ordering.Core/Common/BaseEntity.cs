namespace Ordering.Core.Common
{
    public abstract class BaseEntity
    {
        /* protected set có nghĩa là:
         * Bất kỳ class nào kế thừa BaseEntity đều có quyền gán giá trị cho Id.
         * Các class bên ngoài (không kế thừa) chỉ có thể đọc (get) giá trị Id, không thể gán trực tiếp.
         */
        public int Id { get; protected set; }
        // Optional properties for auditing purposes.
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
