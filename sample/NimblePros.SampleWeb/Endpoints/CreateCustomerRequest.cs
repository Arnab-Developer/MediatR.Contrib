﻿namespace NimblePros.SampleWeb.Endpoints;

public class CreateCustomerRequest
{
  public string FirstName { get; set; } = string.Empty;

  public string LastName { get; set; } = string.Empty;

  public string Country { get; set; } = string.Empty;
}
