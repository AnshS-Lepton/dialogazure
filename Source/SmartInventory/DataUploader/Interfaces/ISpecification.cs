using Models;
using System.Collections.Generic;

namespace DataUploader
{
    public interface ISpecification
    {
        List<KeyValueDropDown> GetSpecification();
        
    }
}