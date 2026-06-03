import { test, expect } from '@playwright/test'

test.describe('Login', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login')
  })

  test('should display login form', async ({ page }) => {
    await expect(page.getByRole('heading', { name: 'Todo App' })).toBeVisible()
    await expect(page.getByLabel('Username')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Sign In' })).toBeVisible()
  })

  test('should login with demo user and redirect to dashboard', async ({ page }) => {
    await page.getByLabel('Username').fill('demo')
    await page.getByRole('button', { name: 'Sign In' }).click()
    await expect(page).toHaveURL('/')
    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible()
  })

  test('should show error for non-existent user', async ({ page }) => {
    await page.getByLabel('Username').fill('nonexistent_user_xyz')
    await page.getByRole('button', { name: 'Sign In' }).click()
    await expect(page.getByText('Failed to authenticate')).toBeVisible()
  })

  test('should disable sign in button when username is empty', async ({ page }) => {
    await expect(page.getByRole('button', { name: 'Sign In' })).toBeDisabled()
  })
})
