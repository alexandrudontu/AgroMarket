import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../../features/auth/auth.service';
import { authGuard } from './auth-guard';

describe('authGuard', () => {
  let authServiceMock: { isLoggedIn: ReturnType<typeof vi.fn> };
  let routerMock: { navigate: ReturnType<typeof vi.fn> };

  const executeGuard = () => TestBed.runInInjectionContext(() =>
    authGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot)
  );

  beforeEach(() => {
    authServiceMock = {
      isLoggedIn: vi.fn()
    };

    routerMock = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    });
  });

  it('allows access when user is logged in', () => {
    authServiceMock.isLoggedIn.mockReturnValue(true);

    const result = executeGuard();

    expect(result).toBe(true);
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('redirects to login when user is not logged in', () => {
    authServiceMock.isLoggedIn.mockReturnValue(false);

    const result = executeGuard();

    expect(result).toBe(false);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });
});
