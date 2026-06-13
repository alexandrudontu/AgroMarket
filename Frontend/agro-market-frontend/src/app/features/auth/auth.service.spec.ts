import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

function createTokenWithRole(role: string): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const payload = btoa(JSON.stringify({
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': role
  }));

  return `${header}.${payload}.signature`;
}

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiUrl}/api/auth`;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('sends login data to the API and receives a text token', () => {
    const credentials = { email: 'user@test.com', password: 'Password123!' };

    service.login(credentials).subscribe(token => {
      expect(token).toBe('jwt-token');
    });

    const request = httpMock.expectOne(`${baseUrl}/login`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(credentials);
    expect(request.request.responseType).toBe('text');

    request.flush('jwt-token');
  });

  it('sends register data to the API', () => {
    const registerData = {
      firstName: 'Alex',
      lastName: 'Popescu',
      email: 'alex@test.com',
      password: 'Password123!',
      accountType: 'Customer'
    };

    service.register(registerData).subscribe(response => {
      expect(response).toEqual({ success: true });
    });

    const request = httpMock.expectOne(`${baseUrl}/register`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(registerData);

    request.flush({ success: true });
  });

  it('saves and reads the authentication token from localStorage', () => {
    service.saveToken('jwt-token');

    expect(service.getToken()).toBe('jwt-token');
    expect(service.isLoggedIn()).toBe(true);
  });

  it('removes the authentication token on logout', () => {
    service.saveToken('jwt-token');

    service.logout();

    expect(service.getToken()).toBeNull();
    expect(service.isLoggedIn()).toBe(false);
  });

  it('extracts the user role from the JWT payload', () => {
    service.saveToken(createTokenWithRole('Farmer'));

    expect(service.getUserRole()).toBe('Farmer');
  });

  it('returns null when no token exists', () => {
    expect(service.getUserRole()).toBeNull();
  });
});
