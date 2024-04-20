{
  inputs.nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";

  outputs = inputs:
    let
      lib = inputs.nixpkgs.lib;
      foreach = xs: f: with lib; foldr recursiveUpdate { } (
        if isList xs then map f xs
        else if isAttrs xs then mapAttrsToList f xs
        else throw "foreach: expected list or attrset, but got ${builtins.typeOf xs}"
      );
    in
    foreach inputs.nixpkgs.legacyPackages (system: pkgs:
      {
        formatter.${system} = pkgs.nixpkgs-fmt;
        devShells.${system}.default = pkgs.mkShell {
          nativeBuildInputs = with pkgs; [
            (with dotnetCorePackages; combinePackages [
              sdk_8_0
            ])
            fsautocomplete
            nodejs_latest
            yarn
          ];
        };
      }
    );
}