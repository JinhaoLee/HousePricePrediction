//google map api

import { Directive, NgModule, InjectionToken, ModuleWithProviders, Inject, Injectable, AfterContentInit, ElementRef, Input, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
const LazyMapAPIConfig = new InjectionToken('errisy-map');

/**confi*/
export interface ILazyMapAPIConfig {
    key?: string;
}

@Injectable()
export class GoogleMapAPILoader {
    constructor( @Inject(LazyMapAPIConfig) private config: ILazyMapAPIConfig) {
        //console.log('GoogleMapAPILoader', config);
    }
    private static callbackName: string = `ErrisyGoogleMapsLazyMapsAPILoader`;

    /**This is the promise that can wait until the google map APIs are loaded. If you want to use or extend any of the google.maps objects, you must await this promise before call the codes. */
    static IsGoogleMapAPILoaded = new Promise<boolean>((resolve, reject) => {
        GoogleMapAPILoader.APILoadedPromiseCallback = { resolve: resolve, reject: reject };
    })

    private static APILoadedPromiseCallback: { resolve: (value:boolean) => any, reject: (reason: any)=>any}

    private static GoogleMapScriptLoadingPromise: Promise<void>;

    load(): Promise<void> {
        if (GoogleMapAPILoader.GoogleMapScriptLoadingPromise) {
            return GoogleMapAPILoader.GoogleMapScriptLoadingPromise;
        }
        //console.log('load promise...');
        const script = document.createElement('script');
        script.type = 'text/javascript';
        script.async = true;
        script.defer = true;
        
        script.src = this._getScriptSrc(GoogleMapAPILoader.callbackName);

        GoogleMapAPILoader.GoogleMapScriptLoadingPromise = new Promise<void>((resolve: Function, reject: Function) => {
            (<any>window)[GoogleMapAPILoader.callbackName] = () => {
                //console.log('google: ', google);
                resolve();
                GoogleMapAPILoader.APILoadedPromiseCallback.resolve(true);
            };
            script.onerror = (error: Event) => {
                reject(error);
                GoogleMapAPILoader.APILoadedPromiseCallback.reject(error);
            };
        });
        document.body.appendChild(script);
        return GoogleMapAPILoader.GoogleMapScriptLoadingPromise;
    }
    private _getScriptSrc(callbackName: string): string {
        //console.log('google map key:', this.config.key);
        return `//maps.googleapis.com/maps/api/js?key=${this.config.key}&callback=${callbackName}`; //key=${this.config.key}&
    }

}

/**
 * this is very thin wrapper of google map, it will emit the (MapInit)="onMapReady($map);"
 * to set up intial values of center and zoom, use [Lat] [Lng] [Zoom]
 */
@Directive({
    selector: '[google-map]',
    providers: [GoogleMapAPILoader] //GoogleMapAPILoader
})
export class GoogleMapDirective implements AfterContentInit{
    constructor(private elementRef: ElementRef, private googleMapAPILoader: GoogleMapAPILoader, private changeDetectorRef: ChangeDetectorRef) {
        //console.log('GoogleMapDirective');
    }
    public Map: google.maps.Map;
    private lat: number = -27.4761628;
    private lng: number = 153.028036;
    private zoom: number = 12;
    @Output() public MapInit: EventEmitter<google.maps.Map> = new EventEmitter<google.maps.Map>();
    @Input() public set Lat(value: number) {
        if (typeof Number(value) == 'number') {
            this.lat = Number(value);
        }
    };
    @Input() public set Lng(value: number) {
        if (typeof Number(value) == 'number') {
            this.lng = Number(value);
        }
    };
    @Input() public set Zoom(value: number) {
        if (typeof Number(value) == 'number') {
            let parsed = Math.round(Number(value));
            if (parsed < 0) parsed = 0;
            if (parsed > 20) parsed = 20;
            this.zoom = parsed;
        }
    }
    public get Lat(): number {
        return this.lat;
    }
    public get Lng(): number {
        return this.lng;
    }
    public get Zoom(): number {
        return this.zoom;
    }
    async ngAfterContentInit() {
        await this.googleMapAPILoader.load();
        //console.log('api loader has been called.');
        this.Map = new google.maps.Map(this.elementRef.nativeElement, {
            center: new google.maps.LatLng(this.lat, this.lng),
            zoom: this.zoom
        });
        //this.Map.
        this.MapInit.emit(this.Map); //emit the map to the parent control
    }
    
}


@NgModule({
    declarations: [GoogleMapDirective],
    providers: [],
    exports: [GoogleMapDirective]
})
export class ErrisyMapModule{
    public static forRoot(lazyMapsAPILoaderConfig?: ILazyMapAPIConfig): ModuleWithProviders {
        return {
            ngModule: ErrisyMapModule,
            providers: [
                { provide: LazyMapAPIConfig, useValue: lazyMapsAPILoaderConfig }
            ],
        };
    }
}