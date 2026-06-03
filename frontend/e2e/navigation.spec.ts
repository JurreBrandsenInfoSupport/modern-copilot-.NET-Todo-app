import { test, expect } from '@playwright/test'

test.describe('Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login')
    await page.getByLabel('Username').fill('demo')
    await page.getByRole('button', { name: 'Sign In' }).click()
    await expect(page).toHaveURL('/')
  })

  test('should navigate to all pages via sidebar', async ({ page }) => {
    // Tasks
    await page.getByRole('link', { name: /tasks/i }).click()
    await expect(page).toHaveURL('/tasks')
    await expect(page.getByRole('heading', { name: 'Tasks' })).toBeVisible()

    // Users
    await page.getByRole('link', { name: /users/i }).click()
    await expect(page).toHaveURL('/users')
    await expect(page.getByRole('heading', { name: 'Users', exact: true }).first()).toBeVisible()

    // Health
    await page.getByRole('link', { name: /health/i }).click()
    await expect(page).toHaveURL('/health')
    await expect(page.getByText('Health Check')).toBeVisible()

    // Dashboard (home)
    await page.getByRole('link', { name: /dashboard/i }).click()
    await expect(page).toHaveURL('/')
    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible()
  })

  test('should show logged-in username', async ({ page }) => {
    await expect(page.getByText('demo')).toBeVisible()
  })

  test('should logout and redirect to login', async ({ page }) => {
    await page.getByRole('button', { name: 'Logout' }).click()
    await expect(page).toHaveURL('/login')
  })
})
