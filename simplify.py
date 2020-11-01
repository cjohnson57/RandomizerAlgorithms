import boolean
import sys
import os
# This script simplifies a boolean logic expression
# So input "A or A and B" will give output "A"
# Used in calculation of world complexity
algebra = boolean.BooleanAlgebra()
requirements = sys.argv[1]
test = algebra.parse(requirements)
print(test.simplify()) # Redirected to a string in the C#
