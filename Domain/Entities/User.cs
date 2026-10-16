using Domain.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

public class User : BaseEntitiy
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<Store> Stores { get; private set; } = new List<Store>();
    public ICollection<Order> Orders { get; private set; } = new List<Order>();

    public ShoppingCart ShoppingCart { get; private set; } = null!;

    public Wallet Wallet { get; private set; }

    public User()
    {
    }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string phoneNumber,
        UserRole role)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (firstName.Length < 3)
            throw new DomainException("First name is too short.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number is required.");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Role = role;
        IsActive = true;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ChangeUserRoleToSeller()
    {
        Role = UserRole.Seller;
    }

    public void UpdateUser(string? phoneNumber, string? email, string? firstName, string? lastName) 
    {
        if (!string.IsNullOrWhiteSpace(phoneNumber)) 
        {
            PhoneNumber = phoneNumber;
        }
        if (!string.IsNullOrWhiteSpace(email)) 
        {
            Email = email;
        }
        if (!string.IsNullOrWhiteSpace(LastName)) 
        {
            LastName = LastName;
        }
        if (!string.IsNullOrWhiteSpace(firstName) && firstName.Length >= 3) 
        {
            FirstName = firstName;
        }
        if (!string.IsNullOrWhiteSpace(lastName))
        {
            LastName = lastName;
        }

    }

    public void UpdatePaswword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException("Password Is Requierd");
        }

        PasswordHash = password;
    }
}