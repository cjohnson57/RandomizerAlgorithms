import boolean
import sys
import os
# This script simplifies a boolean logic expression
# So input "A or A and B" will give output "A"
# Used in calculation of world complexity
algebra = boolean.BooleanAlgebra()
requirements = sys.argv[1]
# If argument is a file, set text to parse to equal file contents
# Used when logical clause is too large to pass as command line argument
if ".txt" in requirements:
    file = open(requirements, mode='r')
    requirements = file.read()
    file.close()
    os.remove(sys.argv[1])
parsed = algebra.parse(requirements)
print(parsed.simplify()) # Redirected to a string in the C# code
