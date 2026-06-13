import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../../features/auth/auth.service';
import { roleGuard } from './role-guard';

describe('roleGuard', () => {
  let authServiceMock: { getUserRole: ReturnType<typeof vi.fn> };
  let routerMock: { navigate: ReturnType<typeof vi.fn> };

  const executeGuard = (roles: string[]) => {
    const route = {
      data: { roles }
    } as unknown as ActivatedRouteSnapshot;

    return TestBed.runInInjectionContext(() =>
      roleGuard(route, {} as RouterStateSnapshot)
    );
  };

  beforeEach(() => {
    authServiceMock = {
      getUserRole: vi.fn()
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

  it('allows access when user role is included in route roles', () => {
    authServiceMock.getUserRole.mockReturnValue('Farmer');

    const result = executeGuard(['Farmer', 'Admin']);

    expect(result).toBe(true);
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('redirects to login when user has no role', () => {
    authServiceMock.getUserRole.mockReturnValue(null);

    const result = executeGuard(['Farmer']);

    expect(result).toBe(false);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('redirects to home when user role is not allowed', () => {
    authServiceMock.getUserRole.mockReturnValue('Customer');

    const result = executeGuard(['Farmer']);

    expect(result).toBe(false);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
  });
});
