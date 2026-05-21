import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../features/auth/user.service';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-farmer-location',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './farmer-location.component.html',
  styleUrls: ['./farmer-location.component.css']
})
export class FarmerLocationComponent {

  isLoading = false;
  locationDetected = false;

  location: any = {
    county: '',
    city: '',
    address: '',
    latitude: null,
    longitude: null
  };

  counties: string[] = [
    'Alba',
    'Arad',
    'Argeș',
    'Bacău',
    'Bihor',
    'Bistrița-Năsăud',
    'Botoșani',
    'Brașov',
    'Brăila',
    'Buzău',
    'Caraș-Severin',
    'Călărași',
    'Cluj',
    'Constanța',
    'Covasna',
    'Dâmbovița',
    'Dolj',
    'Galați',
    'Giurgiu',
    'Gorj',
    'Harghita',
    'Hunedoara',
    'Ialomița',
    'Iași',
    'Ilfov',
    'Maramureș',
    'Mehedinți',
    'Mureș',
    'Neamț',
    'Olt',
    'Prahova',
    'Satu Mare',
    'Sălaj',
    'Sibiu',
    'Suceava',
    'Teleorman',
    'Timiș',
    'Tulcea',
    'Vaslui',
    'Vâlcea',
    'Vrancea',
    'București'
  ];

  citiesByCounty: { [key: string]: string[] } = {
    'Alba': ['Alba Iulia', 'Aiud', 'Blaj', 'Sebeș', 'Cugir'],
    'Arad': ['Arad', 'Ineu', 'Lipova', 'Nădlac', 'Chișineu-Criș'],
    'Argeș': ['Pitești', 'Curtea de Argeș', 'Mioveni', 'Câmpulung', 'Costești'],
    'Bacău': ['Bacău', 'Onești', 'Moinești', 'Comănești', 'Buhuși'],
    'Bihor': ['Oradea', 'Salonta', 'Marghita', 'Beiuș', 'Aleșd'],
    'Bistrița-Năsăud': ['Bistrița', 'Beclean', 'Năsăud', 'Sângeorz-Băi'],
    'Botoșani': ['Botoșani', 'Dorohoi', 'Darabani', 'Săveni'],
    'Brașov': ['Brașov', 'Făgăraș', 'Săcele', 'Codlea', 'Râșnov'],
    'Brăila': ['Brăila', 'Ianca', 'Însurăței', 'Făurei'],
    'Buzău': ['Buzău', 'Râmnicu Sărat', 'Nehoiu', 'Pătârlagele'],
    'Caraș-Severin': ['Reșița', 'Caransebeș', 'Bocșa', 'Oravița', 'Moldova Nouă'],
    'Călărași': ['Călărași', 'Oltenița', 'Budești', 'Fundulea'],
    'Cluj': ['Cluj-Napoca', 'Turda', 'Dej', 'Câmpia Turzii', 'Gherla'],
    'Constanța': ['Constanța', 'Mangalia', 'Medgidia', 'Năvodari', 'Cernavodă'],
    'Covasna': ['Sfântu Gheorghe', 'Târgu Secuiesc', 'Covasna', 'Baraolt'],
    'Dâmbovița': ['Târgoviște', 'Moreni', 'Pucioasa', 'Găești', 'Titu'],
    'Dolj': ['Craiova', 'Băilești', 'Calafat', 'Filiași', 'Dăbuleni'],
    'Galați': ['Galați', 'Tecuci', 'Târgu Bujor', 'Berești'],
    'Giurgiu': ['Giurgiu', 'Bolintin-Vale', 'Mihăilești'],
    'Gorj': ['Târgu Jiu', 'Motru', 'Rovinari', 'Tismana', 'Novaci'],
    'Harghita': ['Miercurea Ciuc', 'Odorheiu Secuiesc', 'Gheorgheni', 'Toplița'],
    'Hunedoara': ['Deva', 'Hunedoara', 'Petroșani', 'Lupeni', 'Vulcan'],
    'Ialomița': ['Slobozia', 'Fetești', 'Urziceni', 'Țăndărei'],
    'Iași': ['Iași', 'Pașcani', 'Hârlău', 'Târgu Frumos'],
    'Ilfov': ['Buftea', 'Otopeni', 'Voluntari', 'Pantelimon', 'Măgurele'],
    'Maramureș': ['Baia Mare', 'Sighetu Marmației', 'Borșa', 'Vișeu de Sus'],
    'Mehedinți': ['Drobeta-Turnu Severin', 'Orșova', 'Strehaia', 'Vânju Mare'],
    'Mureș': ['Târgu Mureș', 'Reghin', 'Sighișoara', 'Târnăveni'],
    'Neamț': ['Piatra Neamț', 'Roman', 'Târgu Neamț', 'Bicaz'],
    'Olt': ['Slatina', 'Caracal', 'Balș', 'Corabia', 'Scornicești'],
    'Prahova': ['Ploiești', 'Câmpina', 'Băicoi', 'Breaza', 'Sinaia'],
    'Satu Mare': ['Satu Mare', 'Carei', 'Negrești-Oaș', 'Tășnad'],
    'Sălaj': ['Zalău', 'Șimleu Silvaniei', 'Jibou', 'Cehu Silvaniei'],
    'Sibiu': ['Sibiu', 'Mediaș', 'Cisnădie', 'Avrig', 'Agnita'],
    'Suceava': ['Suceava', 'Fălticeni', 'Rădăuți', 'Câmpulung Moldovenesc'],
    'Teleorman': ['Alexandria', 'Roșiorii de Vede', 'Turnu Măgurele', 'Zimnicea'],
    'Timiș': ['Timișoara', 'Lugoj', 'Sânnicolau Mare', 'Jimbolia', 'Făget'],
    'Tulcea': ['Tulcea', 'Măcin', 'Babadag', 'Isaccea'],
    'Vaslui': ['Vaslui', 'Bârlad', 'Huși', 'Negrești'],
    'Vâlcea': ['Râmnicu Vâlcea', 'Drăgășani', 'Horezu', 'Băbeni', 'Călimănești', 'Băile Olănești'],
    'Vrancea': ['Focșani', 'Adjud', 'Mărășești', 'Odobești', 'Panciu'],
    'București': ['București']
  };

  constructor(
    private userService: UserService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  onCountyChange() {
    this.location.city = '';
    this.location.latitude = null;
    this.location.longitude = null;
    this.locationDetected = false;
  }

  onCityChange() {
    this.location.latitude = null;
    this.location.longitude = null;
    this.locationDetected = false;
  }

  useCurrentLocation() {

    if (!navigator.geolocation) {
      this.toastr.error('Browserul nu suportă geolocația.');
      return;
    }

    this.isLoading = true;

    navigator.geolocation.getCurrentPosition(
      position => {

        this.location.latitude = position.coords.latitude;
        this.location.longitude = position.coords.longitude;

        this.locationDetected = true;
        this.isLoading = false;

        this.toastr.success('Locația curentă a fost preluată.');

        this.cdr.detectChanges();
      },
      error => {

        console.error(error);

        this.isLoading = false;
        this.locationDetected = false;

        if (error.code === error.PERMISSION_DENIED) {
          this.toastr.warning('Accesul la locație a fost refuzat.');
        } else {
          this.toastr.error('Nu am putut prelua locația curentă.');
        }

        this.cdr.detectChanges();
      }
    );
  }

  saveLocation() {

    if (!this.location.county) {
      this.toastr.warning('Selectează județul.');
      return;
    }

    if (!this.location.city) {
      this.toastr.warning('Selectează localitatea.');
      return;
    }

    const body = {
      county: this.location.county,
      city: this.location.city,
      address: this.location.address,
      latitude: this.location.latitude,
      longitude: this.location.longitude
    };

    this.isLoading = true;

    this.userService.updateFarmerLocation(body)
      .subscribe({
        next: () => {

          this.isLoading = false;

          this.toastr.success('Locația fermei a fost salvată.');

          this.cdr.detectChanges();
        },
        error: err => {

          console.error('LOCATION ERROR FULL:', err);
  console.error('BACKEND RESPONSE:', err.error);

  this.isLoading = false;

  let message = 'Locația nu a putut fi salvată.';

  if (typeof err.error === 'string') {
    message = err.error;
  } else if (err.error?.message) {
    message = err.error.message;
  } else if (err.error?.title) {
    message = err.error.title;
  }

          this.toastr.error(message);

          this.cdr.detectChanges();
        }
      });
  }

  resetLocation() {

    this.location = {
      county: '',
      city: '',
      address: '',
      latitude: null,
      longitude: null
    };

    this.locationDetected = false;

    this.cdr.detectChanges();
  }

  getAvailableCities(): string[] {

    if (!this.location.county) {
      return [];
    }

    return this.citiesByCounty[this.location.county] || [];
  }

  hasCoordinates(): boolean {

    return this.location.latitude !== null &&
           this.location.longitude !== null;
  }

  useFarmerLocation() {
    if (!navigator.geolocation) {
      this.toastr.error(
        'Browserul nu suportă geolocația.'
      );
      return;
    }

    navigator.geolocation.getCurrentPosition(
      position => {

        this.location.latitude = position.coords.latitude;
        this.location.longitude = position.coords.longitude;

        this.toastr.success(
          'Locația curentă a fost preluată.'
        );

        this.cdr.detectChanges();
      },
      error => {
        console.error(error);
        this.toastr.warning(
          'Accesul la locație a fost refuzat.'
        );
      }
    );
  }
}