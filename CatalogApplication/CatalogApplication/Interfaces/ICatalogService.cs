﻿using CatalogApplication.Models;

namespace CatalogApplication.Interfaces;

public interface ICatalogService
{
    string GetPath(Catalog catalog, bool finalslash = false);
}