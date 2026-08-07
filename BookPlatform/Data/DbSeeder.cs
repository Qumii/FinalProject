using BookPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookPlatform.Data
{
    public static class DbSeeder
    {
        public const string AdminEmail = "admin@rafplatform.local";
        public const string AdminPassword = "Admin123!";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var admin = await userManager.FindByEmailAsync(AdminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    DisplayName = "Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, AdminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            if (await context.Books.AnyAsync()) return;

            string Preview(string title, string author) =>
                $"Bu, \"{title}\" kitabının nümunə mətnidir (təqdimat/demo məqsədilə platforma daxilində yazılmışdır, əsərin orijinal mətni deyil). " +
                $"{author} tərəfindən yaradılan dünyaya bu səhifədən qısa bir pəncərə açılır — hekayənin ab-havasını, ritmini və dilini hiss etmək üçün. " +
                "Tam mətni oxumaq üçün kitabın nəşr olunmuş orijinalına müraciət et. Bu bölmə platformanın \"onlayn oxu\" funksionallığını nümayiş etdirir: " +
                "səhifə skrollandıqca oxunma sayğacı yenilənir, rəy və qiymətləndirmə imkanı isə kitabın təfərrüat səhifəsindən əlçatandır.";

            var books = new List<Book>
            {
                new() { Title="Əli və Nino", Author="Qurban Səid", Genre="Roman", Year=1937,
                    Description="Bakıda baş verən, Şərq və Qərb dəyərlərinin toqquşduğu fonda keçən məhəbbət hekayəsi.",
                    CoverImageUrl="/images/covers/book1.svg" },
                new() { Title="1984", Author="George Orwell", Genre="Elmi-fantastika", Year=1949,
                    Description="Total nəzarətin hökm sürdüyü distopik cəmiyyətdə fərdin azadlıq mübarizəsi.",
                    CoverImageUrl="/images/covers/book2.svg" },
                new() { Title="Cinayət və Cəza", Author="Fyodor Dostoyevski", Genre="Roman", Year=1866,
                    Description="Vicdan əzabı və günahın psixoloji dərinliyini araşdıran klassik əsər.",
                    CoverImageUrl="/images/covers/book3.svg" },
                new() { Title="Kür qırağının meşələri", Author="İsmayıl Şıxlı", Genre="Roman", Year=1958,
                    Description="XX əsrin əvvəllərində Azərbaycan kəndinin taleyini əks etdirən epik roman.",
                    CoverImageUrl="/images/covers/book4.svg" },
                new() { Title="Kiçik Şahzadə", Author="Antoine de Saint-Exupéry", Genre="Fəlsəfə", Year=1943,
                    Description="Sadəliyin altında böyük həyat həqiqətlərini gizlədən fəlsəfi nağıl.",
                    CoverImageUrl="/images/covers/book5.svg" },
                new() { Title="Sherlock Holmesin sərgüzəştləri", Author="Arthur Conan Doyle", Genre="Detektiv", Year=1892,
                    Description="Məntiq və müşahidənin ustası Holmesin ən məşhur cinayət araşdırmaları.",
                    CoverImageUrl="/images/covers/book6.svg" },
                new() { Title="Dün", Author="Kazuo Ishiguro", Genre="Roman", Year=1989,
                    Description="Keçmişə boylanan bir xidmətçinin fədakarlıq və peşmançılıq üzərindən həyatını nəql edir.",
                    CoverImageUrl="/images/covers/book7.svg" },
                new() { Title="Sofinin dünyası", Author="Jostein Gaarder", Genre="Fəlsəfə", Year=1991,
                    Description="Fəlsəfə tarixini bir gəncin gözüylə maraqlı bir macəraya çevirən əsər.",
                    CoverImageUrl="/images/covers/book8.svg" },
                new() { Title="Yüz ilin tənhalığı", Author="Gabriel García Márquez", Genre="Roman", Year=1967,
                    Description="Buendía ailəsinin nəsillər boyu davam edən sehrli-realist tarixçəsi.",
                    CoverImageUrl="/images/covers/book9.svg" },
                new() { Title="Dune", Author="Frank Herbert", Genre="Elmi-fantastika", Year=1965,
                    Description="Səhra planeti Arrakisdə hakimiyyət, din və ekologiyanın kəsişdiyi epik əsər.",
                    CoverImageUrl="/images/covers/book10.svg" },
                new() { Title="Anna Karenina", Author="Lev Tolstoy", Genre="Roman", Year=1877,
                    Description="Cəmiyyət qaydaları ilə şəxsi hisslər arasında sıxılan bir qadının faciəsi.",
                    CoverImageUrl="/images/covers/book11.svg" },
                new() { Title="Sapiens", Author="Yuval Noah Harari", Genre="Tarix", Year=2011,
                    Description="İnsan növünün mağara dövründən günümüzə qədər keçdiyi yolun təhlili.",
                    CoverImageUrl="/images/covers/book12.svg" },
                new() { Title="Sükutun səsi", Author="Xəyalə Hüseynova", Genre="Poeziya", Year=2014,
                    Description="Müasir Azərbaycan poeziyasından daxili sükut və özünüdərketmə üzərinə şeirlər toplusu.",
                    CoverImageUrl="/images/covers/book13.svg" },
                new() { Title="Nərmin xanımın xatirələri", Author="Elçin", Genre="Bioqrafiya", Year=1998,
                    Description="XX əsr Azərbaycan ziyalısının həyatı və dövrün mühiti üzərinə bioqrafik qeydlər.",
                    CoverImageUrl="/images/covers/book14.svg" },
                new() { Title="Vicdan", Author="İlyas Əfəndiyev", Genre="Dram", Year=1967,
                    Description="İnsan münasibətlərində vicdan və məsuliyyət məsələlərini araşdıran dram əsəri.",
                    CoverImageUrl="/images/covers/book15.svg" },
                new() { Title="Simfoniya", Author="Şəhriyar Qasımov", Genre="Poeziya", Year=2019,
                    Description="Şəhər həyatı və nostalji arasında salınan körpüləri təsvir edən şeirlər toplusu.",
                    CoverImageUrl="/images/covers/book16.svg" },
                new() { Title="Qərinə", Author="Anar", Genre="Roman", Year=1975,
                    Description="Nəsillər arası dəyər dəyişikliyini bir ailənin taleyi üzərindən izləyən əsər.",
                    CoverImageUrl="/images/covers/book17.svg" },
                new() { Title="Qaranlığın ürəyi", Author="Joseph Conrad", Genre="Roman", Year=1899,
                    Description="Konqo çayı boyunca çıxılan səyahətdə insan təbiətinin qaranlıq üzünün araşdırılması.",
                    CoverImageUrl="/images/covers/book18.svg" },
                new() { Title="Qürur və qərəz", Author="Jane Austen", Genre="Roman", Year=1813,
                    Description="XIX əsr İngiltərəsində sinif fərqləri və məhəbbət arasında baş verən klassik hekayə.",
                    CoverImageUrl="/images/covers/book19.svg" },
                new() { Title="Cəsur yeni dünya", Author="Aldous Huxley", Genre="Elmi-fantastika", Year=1932,
                    Description="Genetik mühəndisliklə idarə olunan \"mükəmməl\" cəmiyyətin qaranlıq tərəflərini göstərən distopiya.",
                    CoverImageUrl="/images/covers/book20.svg" },
                new() { Title="Qatil ov", Author="Elnur Aslanov", Genre="Detektiv", Year=2020,
                    Description="Bakının küçələrində baş verən sirli qətllər zəncirini araşdıran müstəntiqin hekayəsi.",
                    CoverImageUrl="/images/covers/book21.svg" },
                new() { Title="Şərqin işığı", Author="Fatma Kərimli", Genre="Tarix", Year=2017,
                    Description="Orta əsrlər İpək Yolu boyunca elm və mədəniyyət mübadiləsinin tarixi icmalı.",
                    CoverImageUrl="/images/covers/book22.svg" },
                new() { Title="Nitsşe: Həyat və fəlsəfə", Author="Rüstəm Vəliyev", Genre="Bioqrafiya", Year=2016,
                    Description="Fridrix Nitsşenin həyatı və fəlsəfi irsinin əlçatan dildə təqdimatı.",
                    CoverImageUrl="/images/covers/book23.svg" },
                new() { Title="Otello", Author="William Shakespeare", Genre="Dram", Year=1603,
                    Description="Qısqanclıq və etimadın faciəvi nəticələrini göstərən klassik teatr əsəri.",
                    CoverImageUrl="/images/covers/book24.svg" },
            };

            foreach (var b in books)
            {
                b.ContentPreview = Preview(b.Title, b.Author);
                b.CreatedAt = DateTime.UtcNow;
            }

            context.Books.AddRange(books);
            await context.SaveChangesAsync();
        }
    }
}
