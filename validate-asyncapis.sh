#!/bin/sh

set -e

asyncapi config analytics --disable

for f in $( ls ./**/asyncapi*.yaml | grep -v snipped.yaml; ls ./*/*/*.aas.yaml ); do 
    echo lint $f
    asyncapi validate $f
done

