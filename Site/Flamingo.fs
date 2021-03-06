﻿namespace Site
 
open WebSharper

[<JavaScript>]
module Flamingo =
    open WebSharper.ThreeJs
    open WebSharper.JQuery
    open WebSharper.Html.Client
    open WebSharper.JavaScript

    let Main a =
        let renderer = new THREE.WebGLRenderer(
                           WebGLRendererConfiguration(
                               Antialias = true
                           )
                       )

        renderer.SetSize(640, 360)
        renderer.SetClearColor(0xffffff)

        let autoRotate = ref false

        JQuery.Of(a :> Dom.Node).Append(renderer.DomElement).Append(
            (Button [Attr.Style "display: block"] -< [Text "Auto rotate | On"]
             |>! OnClick (fun a _ ->
                     if not !autoRotate
                     then
                        autoRotate := true
                        a.Text <- "Auto rotate | Off"

                     else
                        autoRotate := false
                        a.Text <- "Auto rotate | On"
                 )
            ).Dom
        ) |> ignore

        let scene = new THREE.Scene()
        let light = new THREE.DirectionalLight(0xffffff)

        light.Position.Z <- 128.

        scene.Add(light)

        let flamingo =
            new THREE.Mesh(
                new THREE.Geometry(),
                new THREE.MeshNormalMaterial()
            )
        
        (new THREE.JSONLoader(false)).Load(
            "flamingo.json",
            fun (geometry, _) ->
                flamingo.Geometry <- geometry
                
                scene.Add(flamingo)
        )

        let camera = new THREE.PerspectiveCamera(45., 16./9.)
        let controls = new THREE.TrackballControls(camera)

        camera.Position.Z <- 234.

        //---
        let rec frame () =
            renderer.Render(scene, camera)
            controls.Update()

            if !autoRotate
            then
                flamingo.Rotation.Y <- flamingo.Rotation.Y + 0.01

        Animations.current := frame

        Animations.startIfNotStarted()
        //---

    let Sample =
        Samples.Build()
            .Id("Flamingo")
            .FileName(__SOURCE_FILE__)
            .Keywords(["model"])
            .Render(Main)
            .Create()
