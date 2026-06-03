import { test, expect } from '@playwright/test'

test.describe('Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login')
    await page.getByLabel('Username').fill('demo')
    await page.getByRole('button', { name: 'Sign In' }).click()
    await expect(page).toHaveURL('/')
  })

  test('should display dashboard cards with stats', async ({ page }) => {
    await expect(page.getByText('Total Tasks')).toBeVisible()
    await expect(page.getByText('Completed')).toBeVisible()
    await expect(page.getByText('Pending')).toBeVisible()
    await expect(page.getByRole('main').getByText('Users')).toBeVisible()
  })

  test('should display system health status', async ({ page }) => {
    await expect(page.getByText('System Health')).toBeVisible()
    await expect(page.getByText('Healthy')).toBeVisible({ timeout: 10000 })
  })
})
